using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Mastercard.Developer.OAuth1Signer.Core;
using Mastercard.Developer.OAuth1Signer.Core.Signers;
using RestSharp.Portable;

namespace Mastercard.Developer.OAuth1Signer.RestSharp.Signers
{
    /// <summary>
    /// Utility class for signing RestSharp request objects.
    /// </summary>
    public sealed class RestSharpSigner : BaseSigner
    {
        public RestSharpSigner(string consumerKey, RSA signingKey) : base(consumerKey, signingKey)
        {
        }

        public RestSharpSigner(string consumerKey, RSA signingKey, Encoding encoding) : base(consumerKey, signingKey, encoding)
        {
        }

        public void Sign(Uri baseUri, IRestRequest request)
        {
            if (baseUri == null) throw new ArgumentNullException(nameof(baseUri));
            if (request == null) throw new ArgumentNullException(nameof(request));

            // Build the full request URI
            var fullUri = new StringBuilder();
            fullUri.Append(baseUri.ToString().TrimEnd('/'));
            var resource = request.Resource;
            if (!string.IsNullOrEmpty(resource))
            {
                resource = resource.TrimStart('.');
                resource = resource.TrimStart('/');
                fullUri.Append("/").Append(resource);
            }

            var parameterString = new StringBuilder();
            foreach (var requestParameter in request.Parameters.Where(p => p.Type == ParameterType.QueryString))
            {
                parameterString
                    .Append(parameterString.Length > 0 ? "&" : string.Empty)
                    .Append(requestParameter.Name)
                    .Append("=")
                    .Append(requestParameter.Value);
            }
            if (parameterString.Length > 0)
            {
                fullUri.Append("?").Append(parameterString);
            }

            // Read the body
            var bodyParam = request.Parameters.FirstOrDefault(param => param.Type == ParameterType.RequestBody);
            var payload = bodyParam is null ? string.Empty : bodyParam.Value.ToString();

            // Generate the header and add it to the request
            var methodString = request.Method.ToString();
            var header = OAuth.GetAuthorizationHeader(fullUri.ToString(), methodString, payload, Encoding, ConsumerKey, SigningKey);
            request.AddOrUpdateHeader(OAuth.AuthorizationHeaderName, header);
        }
    }
}