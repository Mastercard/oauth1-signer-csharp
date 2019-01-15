using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Mastercard.Developer.OAuth1Signer.Core.Signers
{
    /// <summary>
    /// Utility class for signing System.Net.Http.HttpClient request objects.
    /// </summary>
    public class NetHttpClientSigner : BaseSigner
    {
        public NetHttpClientSigner(string consumerKey, RSA signingKey) : base(consumerKey, signingKey)
        {
        }

        public NetHttpClientSigner(string consumerKey, RSA signingKey, Encoding encoding) : base(consumerKey, signingKey, encoding)
        {
        }

        public void Sign(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            string payload = null;
            var httpContent = request.Content;
            if (httpContent != null)
            {
                // Read the body
                var bodyTask = httpContent.ReadAsStringAsync();
                bodyTask.Wait();
                payload = bodyTask.Result;
            }

            // Generate the header and add it to the request
            var methodString = request.Method.ToString();
            var header = OAuth.GetAuthorizationHeader(request.RequestUri.AbsoluteUri, methodString, payload, Encoding, ConsumerKey, SigningKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", header.Replace("OAuth", string.Empty));
        }
    }
}
