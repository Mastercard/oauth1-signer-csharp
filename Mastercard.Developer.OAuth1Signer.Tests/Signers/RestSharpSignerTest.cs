using System;
using Mastercard.Developer.OAuth1Signer.RestSharp.Signers;
using Mastercard.Developer.OAuth1Signer.Tests.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp.Portable;

namespace Mastercard.Developer.OAuth1Signer.Tests.Signers
{
    [TestClass]
    public class RestSharpSignerTest
    {
        [TestMethod]
        public void TestSign_ShouldAddOAuth1HeaderToPostRequest()
        {
            // GIVEN
            var signingKey = TestUtils.GetTestSigningKey();
            const string consumerKey = "Some key";
            var baseUri = new Uri("https://api.mastercard.com/");
            var request = new RestRequest
            {
                Method = Method.POST,
                Resource = "/service",
                Parameters =
                {
                    new Parameter { Type = ParameterType.QueryString, Name = "param1", Value = "with spaces" },
                    new Parameter { Type = ParameterType.QueryString, Name = "param2", Value = "encoded#symbol" }
                }
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

        [TestMethod]
        public void TestSign_ShouldAddOAuth1HeaderToGetRequest()
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
            var instanceUnderTest = new RestSharpSigner(consumerKey, signingKey);
            instanceUnderTest.Sign(baseUri, request);

            // THEN
            var authorizationHeaders = request.Parameters.Find(ParameterType.HttpHeader, "Authorization");
            var authorizationHeaderValue = authorizationHeaders[0].Value as string;
            Assert.IsNotNull(authorizationHeaderValue);
        }
    }
}
