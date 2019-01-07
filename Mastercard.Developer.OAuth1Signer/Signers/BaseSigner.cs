using System;
using System.Security.Cryptography;
using System.Text;

namespace Mastercard.Developer.OAuth1Signer.Signers
{
    public abstract class BaseSigner
    {
        protected RSA SigningKey { get; }
        protected string ConsumerKey { get; }
        protected Encoding Encoding { get; }

        protected BaseSigner(string consumerKey, RSA signingKey, Encoding encoding)
        {
            ConsumerKey = consumerKey ?? throw new ArgumentNullException(nameof(consumerKey));
            SigningKey = signingKey ?? throw new ArgumentNullException(nameof(signingKey));
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        protected BaseSigner(string consumerKey, RSA signingKey) : this(consumerKey, signingKey, Encoding.UTF8)
        {
        }
    }
}
