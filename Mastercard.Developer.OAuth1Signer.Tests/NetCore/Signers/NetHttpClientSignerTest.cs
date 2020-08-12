using System;
using System.Net.Http;
using System.Threading.Tasks;

using Mastercard.Developer.OAuth1Signer.Core.Signers;
using Mastercard.Developer.OAuth1Signer.Tests.NetCore.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mastercard.Developer.OAuth1Signer.Tests.NetCore.Signers
{
    [TestClass]
    public class NetHttpClientSignerTest
    {
        #region Sign

        [TestMethod]
        public void TestSign_ShouldAddOAuth1HeaderToPostRequest()
        {
            // GIVEN
            var signingKey = TestUtils.GetTestSigningKey();
            const string consumerKey = "Some key";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.mastercard.com/service"),
                Content = new StringContent("{\"foo\":\"bår\"}") // "application/json; charset=utf-8"
            };

            // WHEN
            var instanceUnderTest = new NetHttpClientSigner(consumerKey, signingKey);
            instanceUnderTest.Sign(request);

            // THEN
            Assert.IsNotNull(request.Headers.Authorization);
        }

        [TestMethod]
        public void TestSign_ShouldAddOAuth1HeaderToGetRequest()
        {
            // GIVEN
            var signingKey = TestUtils.GetTestSigningKey();
            const string consumerKey = "Some key";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.mastercard.com/service")
            };

            // WHEN
            var instanceUnderTest = new NetHttpClientSigner(consumerKey, signingKey);
            instanceUnderTest.Sign(request);

            // THEN
            Assert.IsNotNull(request.Headers.Authorization);
        }

        #endregion

        #region SignAsync

        [TestMethod]
        public async Task TestSignAsync_ShouldAddOAuth1HeaderToPostRequest()
        {
            // GIVEN
            var signingKey = TestUtils.GetTestSigningKey();
            const string consumerKey = "Some key";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.mastercard.com/service"),
                Content = new StringContent("{\"foo\":\"bår\"}") // "application/json; charset=utf-8"
            };

            // WHEN
            var instanceUnderTest = new NetHttpClientSigner(consumerKey, signingKey);
            await instanceUnderTest.SignAsync(request);

            // THEN
            Assert.IsNotNull(request.Headers.Authorization);
        }

        [TestMethod]
        public async Task TestSignAsync_ShouldAddOAuth1HeaderToGetRequest()
        {
            // GIVEN
            var signingKey = TestUtils.GetTestSigningKey();
            const string consumerKey = "Some key";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.mastercard.com/service")
            };

            // WHEN
            var instanceUnderTest = new NetHttpClientSigner(consumerKey, signingKey);
            await instanceUnderTest.SignAsync(request);

            // THEN
            Assert.IsNotNull(request.Headers.Authorization);
        }

        #endregion
    }
}
