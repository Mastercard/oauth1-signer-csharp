using System;
using Mastercard.Developer.OAuth1Signer.Signers;
using Mastercard.Developer.OAuth1SignerTest.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp.Portable;

namespace Mastercard.Developer.OAuth1SignerTest.Signers
{
    [TestClass]
    public class RestSharpSignerTest
    {
        [TestMethod]
        public void TestSign_ShouldAddOAuth1HeaderToRequest()
        {
            // GIVEN
            var signingKey = TestUtils.GetTestPrivateKey();
            const string consumerKey = "Some key";
            var baseUri = new Uri("https://api.mastercard.com/");
            IRestRequest request = new RestRequest
            {
                Method = Method.POST,
                Resource = "/service"
            };
            request.AddJsonBody("{\"foo\":\"bår\"}"); // "application/json; charset=utf-8"

            // WHEN
            var instanceUnderTest = new RestSharpSigner(consumerKey, signingKey);
            instanceUnderTest.Sign(baseUri, request);

            // THEN
            var authorizationHeaders = request.Parameters.Find(ParameterType.HttpHeader, "Authorization");
            var authorizationHeaderValue = authorizationHeaders[0].Value as string;
            Assert.IsNotNull(authorizationHeaderValue);
        }
    }
}
