using System;
using System.Text;
using Mastercard.Developer.OAuth1Signer.RestSharp.Signers;
using Mastercard.Developer.OAuth1Signer.Tests.NetCore2.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp.Portable;

namespace Mastercard.Developer.OAuth1Signer.Tests.NetCore2.Signers
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
                Resource = "/service/{param1}",
                Parameters =
                {
                    new Parameter { Type = ParameterType.UrlSegment, Name = "param1", Value = "value" },
                    new Parameter { Type = ParameterType.QueryString, Name = "param2", Value = "with spaces" },
                    new Parameter { Type = ParameterType.QueryString, Name = "param3", Value = "encoded#symbol" },
                    new Parameter { Type = ParameterType.RequestBody, Encoding = Encoding.UTF8, Value = "{\"foo\":\"bår\"}"}
                }
            };

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

        [TestMethod]
        public void TestSign_ShouldAddOAuthHeaderWhenUrlSegmentParamHasSpecialChars()
        {
            // GIVEN
            var signingKey = TestUtils.GetTestSigningKey();
            const string consumerKey = "Some key";
            var baseUri = new Uri("https://api.mastercard.com/");
            var request = new RestRequest
            {
                Method = Method.GET,
                Resource = "/service/{param1}",
                Parameters =
                {
                     new Parameter { Type = ParameterType.UrlSegment, Name = "param1", Value = "matest.buyer5.pay@track" }
                }
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
