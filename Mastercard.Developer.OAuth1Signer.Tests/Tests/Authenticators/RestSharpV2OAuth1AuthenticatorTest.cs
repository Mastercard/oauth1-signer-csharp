using System;
using Mastercard.Developer.OAuth1Signer.RestSharpV2.Authenticators;
using Mastercard.Developer.OAuth1Signer.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace Mastercard.Developer.OAuth1Signer.Tests.Authenticators
{
    [TestClass]
    public class RestSharpV2OAuth1AuthenticatorTest
    {
        [TestMethod]
        public void TestPreAuthenticate_ShouldSignRequest()
        {
            // GIVEN
            var signingKey = TestUtils.GetTestSigningKey();
            const string consumerKey = "Some key";
            var baseUri = new Uri("https://api.mastercard.com/");
            var request = new RestRequest
            {
                Method = Method.Get,
                Resource = "/service"
            };

            // WHEN
            var instanceUnderTest = new RestSharpOAuth1Authenticator(consumerKey, signingKey, baseUri);
            var task = instanceUnderTest.PreAuthenticate(null, request, null);
            task.Wait();

            // THEN
            var authorizationHeader = request.Parameters.TryFind("Authorization");
            Assert.IsNotNull(authorizationHeader);
        }
    }
}