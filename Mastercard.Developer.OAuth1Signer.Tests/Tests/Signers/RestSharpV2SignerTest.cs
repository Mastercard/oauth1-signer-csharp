using System;
using Mastercard.Developer.OAuth1Signer.RestSharpV2.Signers;
using Mastercard.Developer.OAuth1Signer.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace Mastercard.Developer.OAuth1Signer.Tests.Signers
{
    [TestClass]
    public class RestSharpV2SignerTest
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
                Method = Method.Post,
                Resource = "/service/{param1}"
            };
            
        
            //request.AddHeader("Accept", "application/json");
            //request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;
            request.AddUrlSegment("param1", "value");
            request.AddQueryParameter("param2", "with spaces");
            request.AddQueryParameter("param3", "with spaces");
            //request.AddBody("{\"param4\": {\"foo\":\"bar\"}}");
            request.AddJsonBody("{\"param4\": {\"foo\":\"bar\"}}");

            // WHEN
            var instanceUnderTest = new RestSharpSigner(consumerKey, signingKey);
            instanceUnderTest.Sign(baseUri, request);

            // THEN
            var authorizationHeader = request.Parameters.TryFind("Authorization");
            var authorizationHeaderValue = authorizationHeader.Value as string;
            Assert.IsNotNull(authorizationHeaderValue);
        }
        
        [TestMethod]
        public void TestSign_ShouldAddOAuth1HeaderToGetRequest_WhenParameterValueIsNull()
        {
            // GIVEN
            var signingKey = TestUtils.GetTestSigningKey();
            const string consumerKey = "Some key";
            var baseUri = new Uri("https://api.mastercard.com/");
            var request = new RestRequest
            {
                Method = Method.Get,
                Resource = "/service/{param1}"
            };

            request.AddQueryParameter("name", null);

            // WHEN
            var instanceUnderTest = new RestSharpSigner(consumerKey, signingKey);
            Assert.ThrowsException<InvalidOperationException>(() => instanceUnderTest.Sign(baseUri, request));
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
                Method = Method.Get,
                Resource = "/service"
            };

            // WHEN
            var instanceUnderTest = new RestSharpSigner(consumerKey, signingKey);
            instanceUnderTest.Sign(baseUri, request);

            // THEN
            var authorizationHeader = request.Parameters.TryFind("Authorization");
            var authorizationHeaderValue = authorizationHeader.Value as string;
            Assert.IsNotNull(authorizationHeaderValue);
        }

        [TestMethod]
        public void TestSign_ShouldAddOAuthHeaderWhenUrlSegmentParamHasSpecialChars()
        {
            // GIVEN
            var signingKey = TestUtils.GetTestSigningKey();
            const string consumerKey = "Some key";
            var baseUri = new Uri("https://api.mastercard.com/");
            var request = new RestRequest
            {
                Method = Method.Get,
                Resource = "/service/{name}"
            };

            request.AddUrlSegment("name", "matest.buyer5.pay@track");

            // WHEN
            var instanceUnderTest = new RestSharpSigner(consumerKey, signingKey);
            instanceUnderTest.Sign(baseUri, request);

            // THEN
            var authorizationHeader = request.Parameters.TryFind("Authorization");
            var authorizationHeaderValue = authorizationHeader.Value as string;
            Assert.IsNotNull(authorizationHeaderValue);
        }
    }
}
