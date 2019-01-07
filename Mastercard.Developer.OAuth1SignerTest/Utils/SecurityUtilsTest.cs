using Mastercard.Developer.OAuth1Signer.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mastercard.Developer.OAuth1SignerTest.Utils
{
    [TestClass]
    public class SecurityUtilsTest
    {
        [TestMethod]
        public void TestLoadPrivateKey_ShouldReturnKey()
        {
            // GIVEN
            const string keyContainerPath = "./_Resources/test_key_container.p12";
            const string keyAlias = "mykeyalias";
            const string keyPassword = "Password1";

            // WHEN
            var privateKey = SecurityUtils.LoadPrivateKey(keyContainerPath, keyAlias, keyPassword);

            // THEN
            Assert.AreEqual(2048, privateKey.KeySize);
            Assert.AreEqual("RSA", privateKey.KeyExchangeAlgorithm);
        }
    }
}
