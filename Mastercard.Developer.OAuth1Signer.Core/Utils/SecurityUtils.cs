using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
// ReSharper disable InconsistentNaming

namespace Mastercard.Developer.OAuth1Signer.Core.Utils
{
    /// <summary>
    /// Utility class.
    /// </summary>
    public static class SecurityUtils
    {
        /// <summary>
        /// Load a RSA key out of a PKCS#12 container.
        /// </summary>
        public static RSA LoadPrivateKey(string pkcs12KeyFilePath, string signingKeyAlias, string signingKeyPassword, 
            X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
        {
            if (pkcs12KeyFilePath == null) throw new ArgumentNullException(nameof(pkcs12KeyFilePath));
            var signingCertificate = new X509Certificate2(pkcs12KeyFilePath, signingKeyPassword, keyStorageFlags);
            return signingCertificate.GetRSAPrivateKey();
        }
    }
}
