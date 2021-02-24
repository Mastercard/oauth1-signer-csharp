using System;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Mastercard.Developer.OAuth1Signer.RestSharp.Signers;
using RestSharp.Portable;
#pragma warning disable 1591

namespace Mastercard.Developer.OAuth1Signer.RestSharp.Authenticators
{
    /// <inheritdoc />
    /// <summary>
    /// A RestSharp Portable authenticator for computing and adding an OAuth1 authorization header to HTTP requests.
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

        public Task PreAuthenticate(IRestClient client, IRestRequest request, ICredentials credentials) => Task.Run(() => Signer.Sign(BaseUri, request));

        public Task PreAuthenticate(IHttpClient client, IHttpRequestMessage request, ICredentials credentials) => throw new NotImplementedException();

        public bool CanPreAuthenticate(IRestClient client, IRestRequest request, ICredentials credentials) => true;

        public bool CanPreAuthenticate(IHttpClient client, IHttpRequestMessage request, ICredentials credentials) => false;

        public bool CanHandleChallenge(IHttpClient client, IHttpRequestMessage request, ICredentials credentials, IHttpResponseMessage response) => false;

        public Task HandleChallenge(IHttpClient client, IHttpRequestMessage request, ICredentials credentials, IHttpResponseMessage response) => throw new NotImplementedException();

    }
}
