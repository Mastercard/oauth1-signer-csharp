using System;
using System.Security.Cryptography.X509Certificates;
using Mastercard.Developer.OAuth1Signer.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mastercard.Developer.OAuth1Signer.Tests.NetCore2.Utils
{
    [TestClass]
    [Obsolete("Use AuthenticationUtils instead.")]
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
            const X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable; // https://github.com/dotnet/corefx/issues/14745
            var privateKey = SecurityUtils.LoadPrivateKey(keyContainerPath, keyAlias, keyPassword, flags);

            // THEN
            Assert.AreEqual(2048, privateKey.KeySize);
            Assert.AreEqual("RSA", privateKey.KeyExchangeAlgorithm);
        }
    }
}
