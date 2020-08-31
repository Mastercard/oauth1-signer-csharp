using System;
using Mastercard.Developer.OAuth1Signer.RestSharp.Authenticators;
using Mastercard.Developer.OAuth1Signer.Tests.NetCore.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp.Portable;

namespace Mastercard.Developer.OAuth1Signer.Tests.NetCore.Authenticators
{
    [TestClass]
    public class RestSharpOAuth1AuthenticatorTest
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
                Method = Method.GET,
                Resource = "/service"
            };

            // WHEN
            var instanceUnderTest = new RestSharpOAuth1Authenticator(consumerKey, signingKey, baseUri);
            var task = instanceUnderTest.PreAuthenticate(null, request, null);
            task.Wait();

            // THEN
            var authorizationHeaders = request.Parameters.Find(ParameterType.HttpHeader, "Authorization");
            Assert.AreEqual(1, authorizationHeaders.Count);
        }
    }
}
