using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Mastercard.Developer.OAuth1Signer.Core.Utils;
#pragma warning disable 1591

namespace Mastercard.Developer.OAuth1Signer.Core
{
    /// <summary>
    /// Performs OAuth1.0a compliant signing with body hash support for non-urlencoded content types.
    /// </summary>
    public static class OAuth
    {
        public const string AuthorizationHeaderName = "Authorization";
        private static readonly Random Random = new Random();

        /// <summary>
        /// Creates a Mastercard API compliant OAuth Authorization header.
        /// </summary>
        public static string GetAuthorizationHeader(string uri, string method, string payload, Encoding encoding, string consumerKey, RSA signingKey)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            if (consumerKey == null) throw new ArgumentNullException(nameof(consumerKey));
            if (signingKey == null) throw new ArgumentNullException(nameof(signingKey));

            var queryParameters = ExtractQueryParams(uri);
            var oauthParameters = new Dictionary<string, string>
            {
                { "oauth_consumer_key", consumerKey },
                { "oauth_nonce", GetNonce() },
                { "oauth_timestamp", GetTimestamp() },
                { "oauth_signature_method", "RSA-SHA256" },
                { "oauth_version", "1.0" },
                { "oauth_body_hash", GetBodyHash(payload, encoding) }
            };

            // Compute the OAuth signature
            var oauthParamString = GetOAuthParamString(queryParameters, oauthParameters);
            var baseUri = GetBaseUriString(uri);
            var signatureBaseString = GetSignatureBaseString(baseUri, method, oauthParamString);
            var signature = SignSignatureBaseString(signatureBaseString, encoding, signingKey);
            oauthParameters.Add("oauth_signature", signature);

            // Constructs and returns a valid Authorization header as per https://tools.ietf.org/html/rfc5849#section-3.5.1
            var builder = new StringBuilder();
            foreach (var param in oauthParameters)
            {
                builder
                    .Append(builder.Length == 0 ? "OAuth " : ",")
                    .Append(param.Key)
                    .Append("=\"")
                    .Append(ToUriRfc3986(param.Value))
                    .Append("\"");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Parse query parameters out of the URL. https://tools.ietf.org/html/rfc5849#section-3.4.1.3
        /// </summary>
        internal static Dictionary<string, List<string>> ExtractQueryParams(string uri)
        {
            var queryParamCollection = new Dictionary<string, List<string>>();
            var beginIndex = uri.IndexOf('?');
            if (beginIndex <= 0)
            {
                return queryParamCollection;
            }

            var rawQueryString = uri.Substring(beginIndex);
            var decodedQueryString = Uri.UnescapeDataString(rawQueryString);
            bool mustEncode = !decodedQueryString.Equals(rawQueryString);

            var queryParams = rawQueryString.Split('&', '?');
            foreach (var queryParam in queryParams)
            {
                if (string.IsNullOrEmpty(queryParam))
                {
                    continue;
                }

                var separatorIndex = queryParam.IndexOf('=');
                var key = separatorIndex < 0 ? queryParam : Uri.UnescapeDataString(queryParam.Substring(0, separatorIndex));
                var value = separatorIndex < 0 ? string.Empty : Uri.UnescapeDataString(queryParam.Substring(separatorIndex + 1));
                var encodedKey = mustEncode ? ToUriRfc3986(key) : key;
                var encodedValue = mustEncode ? ToUriRfc3986(value) : value;

                if (!queryParamCollection.ContainsKey(encodedKey))
                {
                    queryParamCollection[encodedKey] = new List<string>();
                }
                queryParamCollection[encodedKey].Add(encodedValue);
            }

            return queryParamCollection;
        }

        /// <summary>
        /// Generates a hash based on request payload as per https://tools.ietf.org/id/draft-eaton-oauth-bodyhash-00.html.
        /// "If the request does not have an entity body, the hash should be taken over the empty string".
        /// </summary>
        internal static string GetBodyHash(string payload, Encoding encoding) => Convert.ToBase64String(Sha256Digest(payload ?? string.Empty, encoding));
        
        /// <summary>
        /// Lexicographically sort all parameters and concatenate them into a string as per https://tools.ietf.org/html/rfc5849#section-3.4.1.3.2.
        /// </summary>
        internal static string GetOAuthParamString(IDictionary<string, List<string>> queryParameters, IDictionary<string, string> oauthParameters)
        {
            var sortedParameters = new SortedDictionary<string, List<string>>(queryParameters, StringComparer.Ordinal);
            foreach (var oauthParameter in oauthParameters)
            {
                sortedParameters[oauthParameter.Key] = new List<string> { oauthParameter.Value };
            }

            // Build the OAuth parameter string 
            var parameterString = new StringBuilder();
            foreach (var parameter in sortedParameters)
            {
                var values = parameter.Value;
                values.Sort(StringComparer.Ordinal); // Keys with same name are sorted by their values
                foreach (var value in values)
                {
                    parameterString
                        .Append(parameterString.Length > 0 ? "&" : string.Empty)
                        .Append(parameter.Key)
                        .Append("=")
                        .Append(value);
                }
            }

            return parameterString.ToString();
        }

        /// <summary>
        /// Normalizes the URL as per https://tools.ietf.org/html/rfc5849#section-3.4.1.2.
        /// </summary>
        internal static string GetBaseUriString(string uriString)
        {
            var uri = new Uri(uriString);
            var lowerCaseScheme = uri.Scheme.ToLower();
            var lowerCaseAuthority = uri.Authority.ToLower();
            var path = uri.AbsolutePath;

            if ("http".Equals(lowerCaseScheme) && uri.Port == 80 || "https".Equals(lowerCaseScheme) && uri.Port == 443)
            {
                // Remove port if it matches the default for scheme
                var index = lowerCaseAuthority.LastIndexOf(':');
                if (index >= 0)
                {
                    lowerCaseAuthority = lowerCaseAuthority.Substring(0, index);
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                path = "/";
            }

            return $"{lowerCaseScheme}://{lowerCaseAuthority}{path}"; // Remove query and fragment
        }

        /// <summary>
        /// Generate a valid signature base string as per https://tools.ietf.org/html/rfc5849#section-3.4.1.
        /// </summary>
        internal static string GetSignatureBaseString(string baseUri, string httpMethod, string oauthParamString)
        {
           return httpMethod.ToUpper()                      // Uppercase HTTP method
                  + "&" + ToUriRfc3986(baseUri)             // Base URI 
                  + "&" + ToUriRfc3986(oauthParamString);   // OAuth parameter string
        }

        /// <summary>
        /// Signs the signature base string using an RSA private key. The methodology is described at
        /// https://tools.ietf.org/html/rfc5849#section-3.4.3 but Mastercard uses the stronger SHA-256 algorithm
        /// as a replacement for the described SHA1 which is no longer considered secure.
        /// </summary>
        internal static string SignSignatureBaseString(string baseString, Encoding encoding, RSA privateKey)
        {
            var hash = Sha256Digest(baseString, encoding);
            var signedHashValue = privateKey.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signedHashValue);
        }
        
        /// <summary>
        /// Percent encodes entities as per https://tools.ietf.org/html/rfc3986.
        /// </summary>
        internal static string ToUriRfc3986(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var escaped = new StringBuilder(Uri.EscapeDataString(input));
            string[] uriRfc3986EscapedChars = {"!", "*", "'", "(", ")"};
            foreach (var escapedChar in uriRfc3986EscapedChars)
            {
                escaped.Replace(escapedChar, UriUtils.HexEscape(escapedChar[0]));
            }
            return escaped.ToString();
        }

        /// <summary>
        /// Returns a cryptographic hash of the given input.
        /// </summary>
        private static byte[] Sha256Digest(string input, Encoding encoding)
        {
            var sha256 = SHA256.Create();
            var inputBytes = encoding.GetBytes(input);
            return sha256.ComputeHash(inputBytes);
        }

        /// <summary>
        /// Generates a 16 char random string for replay protection as per https://tools.ietf.org/html/rfc5849#section-3.3.
        /// </summary>
        internal static string GetNonce() => string.Concat(Enumerable.Range(0, 16).Select(_ => Random.Next(16).ToString("x")));

        /// <summary>
        /// Returns UNIX Timestamp as required per https://tools.ietf.org/html/rfc5849#section-3.3.
        /// </summary>
        private static string GetTimestamp()
        {
            var ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000000;
            return ticks.ToString();
        }
    }
}
