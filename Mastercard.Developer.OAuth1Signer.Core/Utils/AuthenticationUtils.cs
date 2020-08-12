using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
// ReSharper disable InconsistentNaming

namespace Mastercard.Developer.OAuth1Signer.Core.Utils
{
    /// <summary>
    /// Utility class.
    /// </summary>
    public static class AuthenticationUtils
    {
        /// <summary>
        /// Load an RSA key out of a PKCS#12 container.
        /// </summary>
        public static RSA LoadSigningKey(string pkcs12KeyFilePath, string signingKeyAlias, string signingKeyPassword,
            X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
        {
            if (pkcs12KeyFilePath == null) throw new ArgumentNullException(nameof(pkcs12KeyFilePath));
            var signingCertificate = new X509Certificate2(pkcs12KeyFilePath, signingKeyPassword, keyStorageFlags);
            return signingCertificate.GetRSAPrivateKey();
        }

        /// <summary>
        /// Load an RSA key out of a PKCS#12 container.
        /// </summary>
        public static RSA LoadSigningKey(byte[] pkcs12RawData, string signingKeyPassword,
            X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
        {
            if (pkcs12RawData == null) throw new ArgumentNullException(nameof(pkcs12RawData));
            var signingCertificate = new X509Certificate2(pkcs12RawData, signingKeyPassword, keyStorageFlags);
            return signingCertificate.GetRSAPrivateKey();
        }
    }
}
