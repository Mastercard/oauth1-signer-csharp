using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mastercard.Developer.OAuth1Signer;
using Mastercard.Developer.OAuth1SignerTest.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mastercard.Developer.OAuth1SignerTest
{
    [TestClass]
    public class OAuthTest
    {
        [TestMethod]
        public void TestExtractQueryParams_ShouldExtractEncodedParams()
        {
            var queryParams = OAuth.ExtractQueryParams("https://sandbox.api.mastercard.com?param1=plus+value&param2=colon:value&param3=a space~");
            Assert.AreEqual(3, queryParams.Count);
            Assert.IsFalse(new List<string> { "plus%2Bvalue" }.Except(queryParams["param1"]).Any());
            Assert.IsFalse(new List<string> { "colon%3Avalue" }.Except(queryParams["param2"]).Any());
            Assert.IsFalse(new List<string> { "a%20space~" }.Except(queryParams["param3"]).Any());
        }

        [TestMethod]
        public void TestExtractQueryParams_ShouldSupportDuplicateKeysAndEmptyValues()
        {
            var queryParams = OAuth.ExtractQueryParams("https://sandbox.api.mastercard.com/audiences/v1/getcountries?offset=0&offset=1&length=10&empty&odd=");
            Assert.AreEqual(4, queryParams.Count);
            Assert.IsFalse(new List<string> { "10" }.Except(queryParams["length"]).Any());
            Assert.IsFalse(new List<string> { "0", "1" }.Except(queryParams["offset"]).Any());
            Assert.IsFalse(new List<string> { string.Empty }.Except(queryParams["empty"]).Any());
            Assert.IsFalse(new List<string> { string.Empty }.Except(queryParams["odd"]).Any());
        }

        [TestMethod]
        public void TestGetBodyHash()
        {
            Assert.AreEqual("47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=", OAuth.GetBodyHash(string.Empty, Encoding.UTF8));
            Assert.AreEqual("+Z+PWW2TJDnPvRcTgol+nKO3LT7xm8smnsg+//XMIyI=", OAuth.GetBodyHash("{\"foõ\":\"bar\"}", Encoding.UTF8));
        }

        [TestMethod]
        public void TestGetOAuthParamString_ShouldSortParametersAndValues()
        {
            var queryParameters = new Dictionary<string, List<string>>
            {
                { "param2", new List<string> { "hello" } },
                { "first_param", new List<string> { "value", "othervalue" } },
                { "param3", new List<string> { "world" } }
            };
            var oauthParameters = new Dictionary<string, string>
            {
                { "oauth_nonce", "randomnonce" },
                { "oauth_body_hash", "body/hash" }
            };

            var paramString = OAuth.GetOAuthParamString(queryParameters, oauthParameters);
            Assert.AreEqual("first_param=othervalue&first_param=value&oauth_body_hash=body/hash&oauth_nonce=randomnonce&param2=hello&param3=world", paramString);
        }

        [TestMethod]
        public void TestGetBaseUriString_ShouldSupportRfcExamples()
        {
            Assert.AreEqual("https://www.example.net:8080/", OAuth.GetBaseUriString("https://www.example.net:8080"));
            Assert.AreEqual("http://example.com/r%20v/X", OAuth.GetBaseUriString("http://EXAMPLE.COM:80/r%20v/X?id=123"));
        }

        [TestMethod]
        public void TestGetBaseUriString_ShouldRemoveRedundantPorts()
        {
            Assert.AreEqual("https://api.mastercard.com/test", OAuth.GetBaseUriString("https://api.mastercard.com:443/test?query=param"));
            Assert.AreEqual("http://api.mastercard.com/test", OAuth.GetBaseUriString("http://api.mastercard.com:80/test"));
            Assert.AreEqual("https://api.mastercard.com:17443/test", OAuth.GetBaseUriString("https://api.mastercard.com:17443/test?query=param"));
        }

        [TestMethod]
        public void TestGetBaseUriString_ShouldRemoveFragments()
        {
            Assert.AreEqual("https://api.mastercard.com/test", OAuth.GetBaseUriString("https://api.mastercard.com/test?query=param#fragment"));
        }

        [TestMethod]
        public void TestGetBaseUriString_ShouldAddTrailingSlash()
        {
            Assert.AreEqual("https://api.mastercard.com/", OAuth.GetBaseUriString("https://api.mastercard.com"));
        }

        [TestMethod]
        public void TestGetBaseUriString_ShouldUseLowercaseSchemesAndHosts()
        {
            Assert.AreEqual("https://api.mastercard.com/TEST", OAuth.GetBaseUriString("HTTPS://API.MASTERCARD.COM/TEST"));
        }

        [TestMethod]
        public void TestGetSignatureBaseString_Nominal()
        {
            const string method = "POST";
            var queryParameters = new Dictionary<string, List<string>>
            {
                { "param2", new List<string> { "hello" } },
                { "first_param", new List<string> { "value", "othervalue" } }
            };
            var oauthParameters = new Dictionary<string, string>
            {
                { "oauth_nonce", "randomnonce" },
                { "oauth_body_hash", "body/hash" }
            };
            var oauthParamString = OAuth.GetOAuthParamString(queryParameters, oauthParameters);
            var actualSignatureBaseString = OAuth.GetSignatureBaseString("https://api.mastercard.com", method, oauthParamString);
            const string expectedSignatureBaseString = "POST&https%3A%2F%2Fapi.mastercard.com&first_param%3Dothervalue%26first_param%3Dvalue%26oauth_body_hash%3Dbody%2Fhash%26oauth_nonce%3Drandomnonce%26param2%3Dhello";
            Assert.AreEqual(expectedSignatureBaseString, actualSignatureBaseString);
        }

        [TestMethod]
        public void TestSignSignatureBaseString()
        {
            const string expectedSignatureString = "IJeNKYGfUhFtj5OAPRI92uwfjJJLCej3RCMLbp7R6OIYJhtwxnTkloHQ2bgV7fks4GT/A7rkqrgUGk0ewbwIC6nS3piJHyKVc7rvQXZuCQeeeQpFzLRiH3rsb+ZS+AULK+jzDje4Fb+BQR6XmxuuJmY6YrAKkj13Ln4K6bZJlSxOizbNvt+Htnx+hNd4VgaVBeJKcLhHfZbWQxK76nMnjY7nDcM/2R6LUIR2oLG1L9m55WP3bakAvmOr392ulv1+mWCwDAZZzQ4lakDD2BTu0ZaVsvBW+mcKFxYeTq7SyTQMM4lEwFPJ6RLc8jJJ+veJXHekLVzWg4qHRtzNBLz1mA==";
            Assert.AreEqual(expectedSignatureString, OAuth.SignSignatureBaseString("baseString", Encoding.UTF8, TestUtils.GetTestPrivateKey()));
        }

        [TestMethod]
        public void TestGetSignatureBaseString_Integrated()
        {
            var encoding = Encoding.GetEncoding("ISO-8859-1");
            const string body = "<?xml version=\"1.0\" encoding=\"Windows-1252\"?><ns2:TerminationInquiryRequest xmlns:ns2=\"http://mastercard.com/termination\"><AcquirerId>1996</AcquirerId><TransactionReferenceNumber>1</TransactionReferenceNumber><Merchant><Name>TEST</Name><DoingBusinessAsName>TEST</DoingBusinessAsName><PhoneNumber>5555555555</PhoneNumber><NationalTaxId>1234567890</NationalTaxId><Address><Line1>5555 Test Lane</Line1><City>TEST</City><CountrySubdivision>XX</CountrySubdivision><PostalCode>12345</PostalCode><Country>USA</Country></Address><Principal><FirstName>John</FirstName><LastName>Smith</LastName><NationalId>1234567890</NationalId><PhoneNumber>5555555555</PhoneNumber><Address><Line1>5555 Test Lane</Line1><City>TEST</City><CountrySubdivision>XX</CountrySubdivision><PostalCode>12345</PostalCode><Country>USA</Country></Address><DriversLicense><Number>1234567890</Number><CountrySubdivision>XX</CountrySubdivision></DriversLicense></Principal></Merchant></ns2:TerminationInquiryRequest>";
            const string method = "POST";
            const string uri = "https://sandbox.api.mastercard.com/fraud/merchant/v1/termination-inquiry?Format=XML&PageOffset=0&PageLength=10";
            var queryParameters = OAuth.ExtractQueryParams(uri);
            var oauthParameters = new Dictionary<string, string>
            {
                { "oauth_consumer_key", "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" },
                { "oauth_nonce", "1111111111111111111" },
                { "oauth_signature_method", "RSA-SHA256" },
                { "oauth_timestamp", "1111111111" },
                { "oauth_version", "1.0" },
                { "oauth_body_hash", OAuth.GetBodyHash(body, encoding) }
            };
            var oauthParamString = OAuth.GetOAuthParamString(queryParameters, oauthParameters);
            var actualSignatureBaseString = OAuth.GetSignatureBaseString(OAuth.GetBaseUriString(uri), method, oauthParamString);
            const string expectedSignatureBaseString = "POST&https%3A%2F%2Fsandbox.api.mastercard.com%2Ffraud%2Fmerchant%2Fv1%2Ftermination-inquiry&Format%3DXML%26PageLength%3D10%26PageOffset%3D0%26oauth_body_hash%3Dh2Pd7zlzEZjZVIKB4j94UZn%2FxxoR3RoCjYQ9%2FJdadGQ%3D%26oauth_consumer_key%3Dxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx%26oauth_nonce%3D1111111111111111111%26oauth_signature_method%3DRSA-SHA256%26oauth_timestamp%3D1111111111%26oauth_version%3D1.0";
            Assert.AreEqual(expectedSignatureBaseString, actualSignatureBaseString);
        }

        [TestMethod]
        public void TestToUriRfc3986()
        {
            Assert.AreEqual("Format%3DXML", OAuth.ToUriRfc3986("Format=XML"));
            Assert.AreEqual("WhqqH%2BTU95VgZMItpdq78BWb4cE%3D", OAuth.ToUriRfc3986("WhqqH+TU95VgZMItpdq78BWb4cE="));
            Assert.AreEqual("WhqqH%2BTU95VgZMItpdq78BWb4cE%3D%26o", OAuth.ToUriRfc3986("WhqqH+TU95VgZMItpdq78BWb4cE=&o"));
            Assert.AreEqual("WhqqH%2BTU95VgZ~Itpdq78BWb4cE%3D%26o", OAuth.ToUriRfc3986("WhqqH+TU95VgZ~Itpdq78BWb4cE=&o")); // Tilde stays unescaped
        }

        [TestMethod]
        public void TestGetNonce_ShouldHaveLengthOf32()
        {
            var nonce = OAuth.GetNonce();
            Assert.AreEqual(32, nonce.Length);
        }
    }
}
