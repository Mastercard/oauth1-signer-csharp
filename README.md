# oauth1-signer-csharp

[![Build Status](https://travis-ci.org/Mastercard/oauth1-signer-csharp.svg?branch=master)](https://travis-ci.org/Mastercard/oauth1-signer-csharp)
[![NuGet](https://img.shields.io/nuget/v/Mastercard.Developer.OAuth1Signer.Core.svg?label=nuget%20|%20OAuth1Signer.Core)](https://www.nuget.org/packages/Mastercard.Developer.OAuth1Signer.Core/)
[![NuGet](https://img.shields.io/nuget/v/Mastercard.Developer.OAuth1Signer.Core.RestSharp.svg?label=nuget%20|%20OAuth1Signer.RestSharp)](https://www.nuget.org/packages/Mastercard.Developer.OAuth1Signer.Core.RestSharp/)
[![License: MIT](https://img.shields.io/badge/license-MIT-yellow.svg)](https://github.com/Mastercard/oauth1-signer-csharp/blob/master/LICENSE)

## Table of Contents
- [Overview](#overview)
  * [Compatibility](#compatibility)
  * [References](#references)
- [Usage](#usage)
  * [Prerequisites](#prerequisites)
  * [Adding the Libraries to Your Project](#adding-the-libraries-to-your-project)
  * [Creating the OAuth Authorization Header](#creating-the-oauth-authorization-header)
  * [Signing HTTP Client Request Objects](#signing-http-client-request-objects)
  * [Integrating with OpenAPI Generator API Client Libraries](#integrating-with-openapi-generator-api-client-libraries)
  
## Overview <a name="overview"></a>
Zero dependency library for generating a Mastercard API compliant OAuth signature.  

### Compatibility <a name="compatibility"></a>
.NET Frameworks implementing [.NET Standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) 1.3

### References <a name="references"></a>
* [OAuth 1.0a specification](https://tools.ietf.org/html/rfc5849)
* [Body hash extension for non application/x-www-form-urlencoded payloads](https://tools.ietf.org/id/draft-eaton-oauth-bodyhash-00.html)

## Usage <a name="usage"></a>

### Prerequisites <a name="prerequisites"></a>
Before using this library, you will need to set up a project in the [Mastercard Developers Portal](https://developer.mastercard.com). 

As part of this set up, you'll receive credentials for your app:
* A consumer key (displayed on the Mastercard Developer Portal)
* A private request signing key (matching the public certificate displayed on the Mastercard Developer Portal)

```cs
var consumerKey = "<insert consumer key>";
var signingKey = SecurityUtils.LoadPrivateKey(
						"<insert PKCS#12 key file path>", 
						"<insert key alias>", 
						"<insert key password>");
```

### Adding the Libraries to Your Project <a name="adding-the-libraries-to-your-project"></a>

#### Package Manager
```
Install-Package Mastercard.Developer.OAuth1Signer.Core
Install-Package Mastercard.Developer.OAuth1Signer.RestSharp
```

#### .NET CLI
```
dotnet add package Mastercard.Developer.OAuth1Signer.Core
dotnet add package Mastercard.Developer.OAuth1Signer.RestSharp
```

### Creating the OAuth Authorization Header <a name="creating-the-oauth-authorization-header"></a>
The method that does all the heavy lifting is `OAuth.GetAuthorizationHeader`, in the `Mastercard.Developer.OAuth1Signer.Core` package. 
You can call into it directly and as long as you provide the correct parameters, it will return a string that you can add into your request's `Authorization` header.

```cs
var consumerKey = "<insert consumer key>";
var signingKey = SecurityUtils.LoadPrivateKey(
						"<insert PKCS#12 key file path>", 
						"<insert key alias>", 
						"<insert key password>");

var uri = "https://sandbox.api.mastercard.com/service";
var method = "GET";
var payload = "Hello world!";
var encoding = Encoding.UTF8;

var authHeader = OAuth.GetAuthorizationHeader(uri, method, payload, encoding, consumerKey, signingKey);
```

### Signing HTTP Client Request Objects <a name="signing-http-client-request-objects"></a>

Alternatively, you can use helper classes for some of the commonly used HTTP clients.

These classes will modify the provided request object in-place and will add the correct `Authorization` header. Once instantiated with a consumer key and private key, these objects can be reused. 

Usage briefly described below, but you can also refer to the test project for examples. 

+ [RestSharp Portable](#restsharp-portable)

#### RestSharp Portable <a name="restsharp-portable"></a>

The `RestSharpSigner` class is provided by the `Mastercard.Developer.OAuth1Signer.RestSharp` package. 

Usage:
```cs
var baseUri = new Uri("https://api.mastercard.com/");
var request = new RestRequest
{
    Method = Method.POST,
    Resource = "/service"
};
request.AddJsonBody("{\"foo\":\"bår\"}"); // "application/json; charset=utf-8"
var signer = new RestSharpSigner(consumerKey, signingKey);
signer.Sign(baseUri, request);
```

### Integrating with OpenAPI Generator API Client Libraries <a name="integrating-with-openapi-generator-api-client-libraries"></a>

[OpenAPI Generator](https://github.com/OpenAPITools/openapi-generator) generates API client libraries from [OpenAPI Specs](https://github.com/OAI/OpenAPI-Specification). 
It provides generators and library templates for supporting multiple languages and frameworks.

This library will provide you with some request interceptor classes you can use when configuring your API client. These classes will take care of adding the correct `Authorization` header before sending the request.

+ [csharp (targetFramework v5.0)](#csharp-generator-target-framework-v5)

#### csharp (targetFramework v5.0) <a name="csharp-generator-target-framework-v5"></a>

##### OpenAPI Generator

```
java -jar openapi-generator-cli.jar generate -i openapi-spec.yaml -g csharp -c config.json -o out
```
config.json:
```json
{ "targetFramework": "v5.0" }
```

See also: [CONFIG OPTIONS for csharp](https://github.com/OpenAPITools/openapi-generator/blob/master/docs/generators/csharp.md)

##### Usage of the RestSharpOAuth1Authenticator

`RestSharpOAuth1Authenticator` is provided in the `Mastercard.Developer.OAuth1Signer.RestSharp` package. 

```cs
var config = Configuration.Default;
config.BasePath = "https://sandbox.api.mastercard.com";
config.ApiClient.RestClient.Authenticator = new RestSharpOAuth1Authenticator(ConsumerKey, signingKey, new Uri(config.BasePath));
var serviceApi = new ServiceApi(config);
// ...
```
