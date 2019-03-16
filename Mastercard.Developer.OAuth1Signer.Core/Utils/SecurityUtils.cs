using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
// ReSharper disable InconsistentNaming

namespace Mastercard.Developer.OAuth1Signer.Core.Utils
{
    /// <summary>
    /// Utility class.
    /// </summary>
    [Obsolete("Use AuthenticationUtils instead.")]
    public static class SecurityUtils
    {
        public static RSA LoadPrivateKey(string pkcs12KeyFilePath, string signingKeyAlias, string signingKeyPassword, 
            X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
        {
            return AuthenticationUtils.LoadSigningKey(pkcs12KeyFilePath, signingKeyAlias, signingKeyPassword, keyStorageFlags);
        }
    }
}
