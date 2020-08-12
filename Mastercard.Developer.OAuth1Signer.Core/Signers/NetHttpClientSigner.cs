using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable 1591

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
            var signTask = SignPrivateAsync(request);
            signTask.Wait();
        }

        public Task SignAsync(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return SignPrivateAsync(request);
        }

        private async Task SignPrivateAsync(HttpRequestMessage request)
        {
            string payload = null;
            var httpContent = request.Content;
            if (httpContent != null)
            {
                // Read the body
                payload = await httpContent.ReadAsStringAsync();
            }

            // Generate the header and add it to the request
            var methodString = request.Method.ToString();
            var header = OAuth.GetAuthorizationHeader(request.RequestUri.AbsoluteUri, methodString, payload, Encoding, ConsumerKey, SigningKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", header.Replace("OAuth", string.Empty));
        }
    }
}
