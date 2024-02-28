using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Mastercard.Developer.OAuth1Signer.RestSharpV2.Signers;
using RestSharp;
using RestSharp.Authenticators;

#pragma warning disable 1591

namespace Mastercard.Developer.OAuth1Signer.RestSharpV2.Authenticators
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

        public Task PreAuthenticate(RestClient client, RestRequest request, ICredentials credentials) => Task.Run(() => Signer.Sign(BaseUri, request));

        public Task PreAuthenticate(HttpClient client, HttpRequestMessage request, ICredentials credentials) => throw new NotImplementedException();

        public bool CanPreAuthenticate(RestClient client, RestRequest request, ICredentials credentials) => true;

        public bool CanPreAuthenticate(HttpClient client, HttpRequestMessage request, ICredentials credentials) => false;

        public bool CanHandleChallenge(HttpClient client, HttpRequestMessage request, ICredentials credentials, HttpResponseMessage response) => false;

        public Task HandleChallenge(HttpClient client, HttpRequestMessage request, ICredentials credentials, HttpResponseMessage response) => throw new NotImplementedException();

        ValueTask IAuthenticator.Authenticate(IRestClient client, RestRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
