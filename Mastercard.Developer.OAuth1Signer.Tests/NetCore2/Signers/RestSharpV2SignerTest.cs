using System;
using Mastercard.Developer.OAuth1Signer.RestSharpV2.Signers;
using Mastercard.Developer.OAuth1Signer.Tests.NetCore.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace Mastercard.Developer.OAuth1Signer.Tests.NetCore.Signers
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
                Method = Method.POST,
                Resource = "/service/{param1}",
                Parameters =
                {
                    new Parameter("param1", "value",  ParameterType.UrlSegment),
                    new Parameter("param2", "with spaces", ParameterType.QueryString),
                    new Parameter("param3", "encoded#symbol", ParameterType.QueryString),
                    new Parameter("param4", "{\"foo\":\"bår\"}", ParameterType.RequestBody)
                }
            };

            // WHEN
            var instanceUnderTest = new RestSharpSigner(consumerKey, signingKey);
            instanceUnderTest.Sign(baseUri, request);

            // THEN
            var authorizationHeaders = request.Parameters.FindAll(param => param.Name == "Authorization" && param.Type == ParameterType.HttpHeader);
            var authorizationHeaderValue = authorizationHeaders[0].Value as string;
            Assert.IsNotNull(authorizationHeaderValue);
        }
        
        [TestMethod]
        public void TestSign_ShouldAddOAuth1HeaderToGetRequest_WhenParameterNameIsNull()
        {
            // GIVEN
            var signingKey = TestUtils.GetTestSigningKey();
            const string consumerKey = "Some key";
            var baseUri = new Uri("https://api.mastercard.com/");
            var param1 = new Parameter("name", "value", ParameterType.QueryString);
            param1.Name = null;
            var request = new RestRequest
            {
                Method = Method.GET,
                Resource = "/service/{param1}",
                Parameters =
                {
                    param1
                }
            };

            // WHEN
            var instanceUnderTest = new RestSharpSigner(consumerKey, signingKey);
            Assert.ThrowsException<InvalidOperationException>(() => instanceUnderTest.Sign(baseUri, request));
        }
        
        [TestMethod]
        public void TestSign_ShouldAddOAuth1HeaderToGetRequest_WhenParameterValueIsNull()
        {
            // GIVEN
            var signingKey = TestUtils.GetTestSigningKey();
            const string consumerKey = "Some key";
            var baseUri = new Uri("https://api.mastercard.com/");
            var param1 = new Parameter("name", "value", ParameterType.QueryString);
            param1.Value = null;
            var request = new RestRequest
            {
                Method = Method.GET,
                Resource = "/service/{param1}",
                Parameters =
                {
                    param1
                }
            };

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
                Method = Method.GET,
                Resource = "/service"
            };

            // WHEN
            var instanceUnderTest = new RestSharpSigner(consumerKey, signingKey);
            instanceUnderTest.Sign(baseUri, request);

            // THEN
            var authorizationHeaders = request.Parameters.FindAll(param => param.Name == "Authorization" && param.Type == ParameterType.HttpHeader);
            var authorizationHeaderValue = authorizationHeaders[0].Value as string;
            Assert.IsNotNull(authorizationHeaderValue);
        }
    }
}
