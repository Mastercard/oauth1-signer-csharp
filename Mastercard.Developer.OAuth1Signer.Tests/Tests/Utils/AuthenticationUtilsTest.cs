using System.IO;
using System.Security.Cryptography.X509Certificates;
using Mastercard.Developer.OAuth1Signer.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mastercard.Developer.OAuth1Signer.Tests.Utils
{
    [TestClass]
    public class AuthenticationUtilsTest
    {
        [TestMethod]
        public void TestLoadSigningKey_ShouldReturnKey()
        {
            // GIVEN
            const string keyContainerPath = "./_Resources/test_key_container.p12";
            const string keyAlias = "mykeyalias";
            const string keyPassword = "Password1";

            // WHEN
            const X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable; // https://github.com/dotnet/corefx/issues/14745
            var privateKey = AuthenticationUtils.LoadSigningKey(keyContainerPath, keyAlias, keyPassword, flags);

            // THEN
            Assert.AreEqual(2048, privateKey.KeySize);
            Assert.AreEqual("RSA", privateKey.KeyExchangeAlgorithm);
        }

        [TestMethod]
        public void TestLoadSigningKey_WithValidByteArray_ShouldReturnKey()
        {
            // GIVEN
            const string keyContainerPath = "./_Resources/test_key_container.p12";
            const string keyPassword = "Password1";

            var p12Bytes = File.ReadAllBytes(keyContainerPath);

            // WHEN
            const X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable; // https://github.com/dotnet/corefx/issues/14745
            var privateKey = AuthenticationUtils.LoadSigningKey(p12Bytes, keyPassword, flags);

            // THEN
            Assert.AreEqual(2048, privateKey.KeySize);
            Assert.AreEqual("RSA", privateKey.KeyExchangeAlgorithm);
        }
    }
}
