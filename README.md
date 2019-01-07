# oauth1-signer-csharp

[![License: MIT](https://img.shields.io/badge/license-MIT-yellow.svg)](https://github.com/Mastercard/oauth1-signer-csharp/blob/master/LICENSE)

## Table of Contents
- [Overview](#overview)
  * [Compatibility](#compatibility)
  * [References](#references)
- [Usage](#usage)
  * [Prerequisites](#prerequisites)
  * [Build and Test the Project](#build-and-test-the-project)
  
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

### Build and Test the Project <a name="build-and-test-the-project"></a>	
```
dotnet test -c Release
```