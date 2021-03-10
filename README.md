# oauth1-signer-csharp

[![](https://github.com/Mastercard/oauth1-signer-csharp/workflows/Build%20&%20Test/badge.svg)](https://github.com/Mastercard/oauth1-signer-csharp/actions?query=workflow%3A%22Build+%26+Test%22)
[![](https://sonarcloud.io/api/project_badges/measure?project=Mastercard_oauth1-signer-csharp&metric=alert_status)](https://sonarcloud.io/dashboard?id=Mastercard_oauth1-signer-csharp)
[![](https://github.com/Mastercard/oauth1-signer-csharp/workflows/broken%20links%3F/badge.svg)](https://github.com/Mastercard/oauth1-signer-csharp/actions?query=workflow%3A%22broken+links%3F%22)
[![](https://img.shields.io/nuget/v/Mastercard.Developer.OAuth1Signer.Core.svg?label=nuget%20|%20core)](https://www.nuget.org/packages/Mastercard.Developer.OAuth1Signer.Core/)
[![](https://img.shields.io/nuget/v/Mastercard.Developer.OAuth1Signer.RestSharp.svg?label=nuget%20|%20restsharp%20portable)](https://www.nuget.org/packages/Mastercard.Developer.OAuth1Signer.RestSharp/)
[![](https://img.shields.io/nuget/v/Mastercard.Developer.OAuth1Signer.RestSharpV2.svg?label=nuget%20|%20restsharp)](https://www.nuget.org/packages/Mastercard.Developer.OAuth1Signer.RestSharpV2/)
[![](https://img.shields.io/badge/license-MIT-yellow.svg)](https://github.com/Mastercard/oauth1-signer-csharp/blob/master/LICENSE)

## Table of Contents
- [Overview](#overview)
  * [Compatibility](#compatibility)
  * [References](#references)
- [Usage](#usage)
  * [Prerequisites](#prerequisites)
  * [Adding the Libraries to Your Project](#adding-the-libraries-to-your-project)
  * [Loading the Signing Key](#loading-the-signing-key) 
  * [Creating the OAuth Authorization Header](#creating-the-oauth-authorization-header)
  * [Signing HTTP Client Request Objects](#signing-http-client-request-objects)
  * [Integrating with OpenAPI Generator API Client Libraries](#integrating-with-openapi-generator-api-client-libraries)
  
## Overview <a name="overview"></a>
* `OAuth1Signer.Core` is a zero dependency library for generating a Mastercard API compliant OAuth signature
* `OAuth1Signer.RestSharpV2` is an extension dedicated to [RestSharp](https://restsharp.dev/)
* `OAuth1Signer.RestSharp` is an extension dedicated to [RestSharp Portable](https://github.com/FubarDevelopment/restsharp.portable) (project no longer maintained)

### Compatibility <a name="compatibility"></a>

#### .NET <a name="net"></a>
* `OAuth1Signer.Core` and `OAuth1Signer.RestSharp` require a .NET Framework implementing [.NET Standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) 1.3.
* `OAuth1Signer.RestSharpV2` requires a .NET Framework implementing [.NET Standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) 2.0.

#### Strong Naming <a name="strong-naming"></a>
Assemblies are strong-named as per [Strong naming and .NET libraries](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/strong-naming).
The SN key is available here: [`Identity.snk`](https://github.com/Mastercard/oauth1-signer-csharp/blob/master/Identity.snk).

### References <a name="references"></a>
* [OAuth 1.0a specification](https://tools.ietf.org/html/rfc5849)
* [Body hash extension for non application/x-www-form-urlencoded payloads](https://tools.ietf.org/id/draft-eaton-oauth-bodyhash-00.html)

## Usage <a name="usage"></a>

### Prerequisites <a name="prerequisites"></a>
Before using this library, you will need to set up a project in the [Mastercard Developers Portal](https://developer.mastercard.com). 

As part of this set up, you'll receive credentials for your app:
* A consumer key (displayed on the Mastercard Developer Portal)
* A private request signing key (matching the public certificate displayed on the Mastercard Developer Portal)

### Adding the Libraries to Your Project <a name="adding-the-libraries-to-your-project"></a> 
#### Package Manager
```shell
Install-Package Mastercard.Developer.OAuth1Signer.{Core|RestSharp|RestSharpV2}
```

#### .NET CLI
```shell
dotnet add package Mastercard.Developer.OAuth1Signer.{Core|RestSharp|RestSharpV2}
```

### Loading the Signing Key <a name="loading-the-signing-key"></a>

A `System.Security.Cryptography.RSA` key object can be created by calling the `AuthenticationUtils.LoadSigningKey` method:
```cs
var signingKey = AuthenticationUtils.LoadSigningKey(
                        "<insert PKCS#12 key file path>", 
                        "<insert key alias>", 
                        "<insert key password>");
```

### Creating the OAuth Authorization Header <a name="creating-the-oauth-authorization-header"></a>
The method that does all the heavy lifting is `OAuth.GetAuthorizationHeader`, in the `OAuth1Signer.Core` package. 
You can call into it directly and as long as you provide the correct parameters, it will return a string that you can add into your request's `Authorization` header.

```cs
var consumerKey = "<insert consumer key>";
var uri = "https://sandbox.api.mastercard.com/service";
var method = "POST";
var payload = "Hello world!";
var encoding = Encoding.UTF8;
var authHeader = OAuth.GetAuthorizationHeader(uri, method, payload, encoding, consumerKey, signingKey);
```

### Signing HTTP Client Request Objects <a name="signing-http-client-request-objects"></a>

Alternatively, you can use helper classes for some of the commonly used HTTP clients.

These classes will modify the provided request object in-place and will add the correct `Authorization` header. Once instantiated with a consumer key and private key, these objects can be reused. 

Usage briefly described below, but you can also refer to the test project for examples. 

+ [System.Net.Http.HttpClient](#system-net-http-httpclient)
+ [RestSharp](#restsharp)

#### System.Net.Http.HttpClient <a name="system-net-http-httpclient"></a>

The `NetHttpClientSigner` class is located in the `OAuth1Signer.Core` package. 

Usage:
```cs
var baseUri = new Uri("https://api.mastercard.com/");
var httpClient = new HttpClient(new RequestSignerHandler(consumerKey, signingKey)) { BaseAddress = baseUri };
var postTask = httpClient.PostAsync(new Uri("/service", UriKind.Relative), new StringContent("{\"foo\":\"bår\"}");
// (...)

internal class RequestSignerHandler : HttpClientHandler
{
    private readonly NetHttpClientSigner signer;

    public RequestSignerHandler(string consumerKey, RSA signingKey)
    {
        signer = new NetHttpClientSigner(consumerKey, signingKey);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        signer.Sign(request);
        return base.SendAsync(request, cancellationToken);
    }
}
```

#### RestSharp <a name="restsharp"></a>

A `RestSharpSigner` class is provided for both RestSharp and RestSharp Portable. It can be found in the `OAuth1Signer.RestSharp` and `OAuth1Signer.RestSharpV2` packages.

Usage:
```cs
var baseUri = new Uri("https://api.mastercard.com/");
var request = new RestRequest
{
    Method = Method.POST,
    Resource = "/service",
    Parameters =
    {
        new Parameter { Type = ParameterType.RequestBody, Encoding = Encoding.UTF8, Value = "{\"foo\":\"bår\"}"} // "application/json; charset=utf-8"
    }
};
var signer = new RestSharpSigner(consumerKey, signingKey);
signer.Sign(baseUri, request);
```

### Integrating with OpenAPI Generator API Client Libraries <a name="integrating-with-openapi-generator-api-client-libraries"></a>

[OpenAPI Generator](https://github.com/OpenAPITools/openapi-generator) generates API client libraries from [OpenAPI Specs](https://github.com/OAI/OpenAPI-Specification). 
It provides generators and library templates for supporting multiple languages and frameworks.

This project provides you with some authenticator classes you can use when configuring your API client. These classes will take care of adding the correct `Authorization` header before sending the request.

Generators currently supported:
+ [csharp-netcore](#csharp-netcore-generator)
+ [csharp (deprecated)](#csharp-generator)


#### csharp-netcore<a name="csharp-netcore-generator"></a>

##### OpenAPI Generator

Client libraries can be generated using the following command:
```shell
openapi-generator-cli generate -i openapi-spec.yaml -g csharp-netcore -c config.json -o out
```
config.json:
```json
{ "targetFramework": "netstandard2.0" }
```

See also:
* [OpenAPI Generator (executable)](https://mvnrepository.com/artifact/org.openapitools/openapi-generator-cli)
* [Config Options for csharp-netcore](https://github.com/OpenAPITools/openapi-generator/blob/master/docs/generators/csharp-netcore.md)

##### Usage of the `RestSharpSigner`

`RestSharpSigner` is located in the `OAuth1Signer.RestSharpV2` package.

##### Usage

Create a new file (for instance, `ApiClientInterceptor.cs`) extending the definition of the generated `ApiClient` class:

```cs
partial class ApiClient
{
    private Uri BasePath { get; }
    private RestSharpSigner Signer { get; }
    
    public ApiClient(RSA signingKey, string basePath, string consumerKey)
    {
        this._baseUrl = basePath;
        this.BasePath = new Uri(basePath);
        this.Signer = new RestSharpSigner(consumerKey, signingKey);
    }

    partial void InterceptRequest(IRestRequest request) => Signer.Sign(this.BasePath, request);
}
```

Configure your `ApiClient` instance the following way:
```cs
var serviceApi = new ServiceApi();
serviceApi.Client = new ApiClient(SigningKey, BasePath, ConsumerKey);
// ...
```

#### csharp (deprecated)<a name="csharp-generator"></a>

##### OpenAPI Generator

Client libraries can be generated using the following command:
```shell
openapi-generator-cli generate -i openapi-spec.yaml -g csharp -c config.json -o out
```
config.json:
```json
{ "targetFramework": "netstandard1.3" }
```

⚠️ `v5.0` was used for `targetFramework` in OpenAPI Generator versions prior 5.0.0.

See also: 
* [OpenAPI Generator (executable)](https://mvnrepository.com/artifact/org.openapitools/openapi-generator-cli)
* [Config Options for csharp](https://github.com/OpenAPITools/openapi-generator/blob/master/docs/generators/csharp.md)

##### Usage of the `RestSharpOAuth1Authenticator`

`RestSharpOAuth1Authenticator` is located in the `OAuth1Signer.RestSharp` package. 

```cs
var config = Configuration.Default;
config.BasePath = "https://sandbox.api.mastercard.com";
config.ApiClient.RestClient.Authenticator = new RestSharpOAuth1Authenticator(ConsumerKey, signingKey, new Uri(config.BasePath));
var serviceApi = new ServiceApi(config);
// ...
```
