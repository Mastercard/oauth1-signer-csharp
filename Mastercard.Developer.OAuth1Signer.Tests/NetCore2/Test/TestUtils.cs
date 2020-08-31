using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mastercard.Developer.OAuth1Signer.Core.Utils;

// ReSharper disable InconsistentNaming

namespace Mastercard.Developer.OAuth1Signer.Tests.NetCore.Test
{
    internal static class TestUtils
    {
        internal static RSA GetTestSigningKey() => AuthenticationUtils.LoadSigningKey(
            "./_Resources/test_key_container.p12", 
            "mykeyalias", 
            "Password1",
            X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable // https://github.com/dotnet/corefx/issues/14745
        );
    }
}
