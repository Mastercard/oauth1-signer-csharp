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

        private static NetHttpClientSigner CreateHttpClientSigner()
        {
            var signingKey = TestUtils.GetTestSigningKey();
            const string consumerKey = "Some key";
            return new NetHttpClientSigner(consumerKey, signingKey);
        }

        private static HttpRequestMessage CreateTestGetRequest()
        {
            return new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.mastercard.com/service")
            };
        }

        private static HttpRequestMessage CreateTestPostRequest()
        {
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.mastercard.com/service"),
                Content = new StringContent("{\"foo\":\"bår\"}") // "application/json; charset=utf-8"
            };
        }

        #region Sign

        [TestMethod]
        public void TestSign_ShouldAddOAuth1HeaderToPostRequest()
        {
            // GIVEN
            var request = CreateTestPostRequest();

            // WHEN
            CreateHttpClientSigner().Sign(request);

            // THEN
            Assert.IsNotNull(request.Headers.Authorization);
        }

        [TestMethod]
        public void TestSign_ShouldAddOAuth1HeaderToGetRequest()
        {
            // GIVEN
            var request = CreateTestGetRequest();

            // WHEN
            CreateHttpClientSigner().Sign(request);

            // THEN
            Assert.IsNotNull(request.Headers.Authorization);
        }

        [TestMethod]
        public void TestSign_ShouldThrowArgumentNullException_WhenNullRequest()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                CreateHttpClientSigner().Sign(null);
            });
        }

        #endregion

        #region SignAsync

        [TestMethod]
        public async Task TestSignAsync_ShouldAddOAuth1HeaderToPostRequest()
        {
            // GIVEN
            var request = CreateTestPostRequest();

            // WHEN
            await CreateHttpClientSigner().SignAsync(request);

            // THEN
            Assert.IsNotNull(request.Headers.Authorization);
        }

        [TestMethod]
        public async Task TestSignAsync_ShouldAddOAuth1HeaderToGetRequest()
        {
            // GIVEN
            var request = CreateTestGetRequest();

            // WHEN
            await CreateHttpClientSigner().SignAsync(request);

            // THEN
            Assert.IsNotNull(request.Headers.Authorization);
        }

        [TestMethod]
        public void TestSignAsync_ShouldThrowArgumentNullException_WhenNullRequest()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                CreateHttpClientSigner().SignAsync(null);
            });
        }

        #endregion
    }
}
