using System.Security.Cryptography;
using Mastercard.Developer.OAuth1Signer.Utils;

namespace Mastercard.Developer.OAuth1SignerTest.Test
{
    internal static class TestUtils
    {
        internal static RSA GetTestPrivateKey() => SecurityUtils.LoadPrivateKey("./_Resources/test_key_container.p12", "mykeyalias", "Password1");
    }
}
