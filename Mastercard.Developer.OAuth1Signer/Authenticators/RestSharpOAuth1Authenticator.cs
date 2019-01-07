using System;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Mastercard.Developer.OAuth1Signer.Signers;
using RestSharp.Portable;

namespace Mastercard.Developer.OAuth1Signer.Authenticators
{
    /// <inheritdoc />
    /// <summary>
    /// A RestSharp authenticator for computing and adding an OAuth1 authorization header to HTTP requests.
    /// </summary>
    public class RestSharpOAuth1Authenticator : IAuthenticator
    {
        private Uri BaseUri { get; }
        private RestSharpSigner Signer { get; }

        public RestSharpOAuth1Authenticator(string consumerKey, RSA signingKey, Uri baseUri)
        {
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            Signer = new RestSharpSigner(consumerKey, signingKey);
        }

        public Task PreAuthenticate(IRestClient client, IRestRequest restRequest, ICredentials credentials) => Task.Run(() => Signer.Sign(BaseUri, restRequest));

        public bool CanPreAuthenticate(IRestClient client, IRestRequest request, ICredentials credentials) => true;

        public bool CanPreAuthenticate(IHttpClient client, IHttpRequestMessage request, ICredentials credentials) => false;

        public bool CanHandleChallenge(IHttpClient client, IHttpRequestMessage request, ICredentials credentials, IHttpResponseMessage response) => false;

        public Task PreAuthenticate(IHttpClient client, IHttpRequestMessage request, ICredentials credentials) => throw new NotImplementedException();

        public Task HandleChallenge(IHttpClient client, IHttpRequestMessage request, ICredentials credentials, IHttpResponseMessage response) => throw new NotImplementedException();

    }
}
