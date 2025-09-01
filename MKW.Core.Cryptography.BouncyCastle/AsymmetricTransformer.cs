using MKW.Core.Common;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace MKW.Core.Cryptography.BouncyCastle
{
    public class AsymmetricTransformer
        : IAsymmetricPrivateTransformer
        , IAsymmetricPublicTransformer
        , IDisposable
    {
        private readonly IBufferedCipher cipher;
        private readonly ISigner signer;

        private readonly AsymmetricKeyParameter publicKey;
        private readonly AsymmetricKeyParameter? privateKey;

        public AsymmetricTransformer(AsymmetricKeyParameter publicKey, AsymmetricKeyParameter? privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;

            cipher = CipherUtilities.GetCipher(PkcsObjectIdentifiers.RsaEncryption);
            signer = SignerUtilities.GetSigner(PkcsObjectIdentifiers.Sha256WithRsaEncryption);
        }

        public static AsymmetricTransformer Create()
        {
            SecureRandom random = new SecureRandom();
            IAsymmetricCipherKeyPairGenerator keyPairGen = GeneratorUtilities.GetKeyPairGenerator("RSA");

            keyPairGen.Init(new KeyGenerationParameters(random, 2048));

            AsymmetricCipherKeyPair key = keyPairGen.GenerateKeyPair();

            return new AsymmetricTransformer(key.Public, key.Private);
        }

        public static AsymmetricTransformer Open(ReadOnlySpan<byte> publicKey)
        {
            return new AsymmetricTransformer(PublicKeyFactory.CreateKey(publicKey.ToArray()),
                                             null);
        }

        public static AsymmetricTransformer Open(ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> privateKey)
        {
            // todo: verify keypair
            return new AsymmetricTransformer(PublicKeyFactory.CreateKey(publicKey.ToArray()),
                                             PrivateKeyFactory.CreateKey(privateKey.ToArray()));
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            cipher.Init(true, publicKey);

            using MemoryStream output = new MemoryStream();

            using (CipherStream cipherStream = new CipherStream(new StreamDisown(output),
                                                                null, cipher))
            {
                cipherStream.Write(data);
            }

            return output.ToArray();
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            if (privateKey == null)
            {
                throw new NotSupportedException("Decryption requires private key.");
            }

            cipher.Init(false, privateKey);

            using MemoryStream output = new MemoryStream();

            using (CipherStream cipherStream = new CipherStream(new StreamDisown(output),
                                                                null, cipher))
            {
                cipherStream.Write(data);
            }

            return output.ToArray();
        }

        public Memory<byte> ExportPrivateKey()
        {
            PrivateKeyInfo info = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
            return info.GetEncoded();
        }

        public Memory<byte> ExportPublicKey()
        {
            SubjectPublicKeyInfo info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            return info.GetEncoded();
        }

        public Memory<byte> Sign(ReadOnlySpan<byte> data)
        {
            signer.Init(true, privateKey);
            signer.BlockUpdate(data);
            return signer.GenerateSignature();
        }

        public bool Verify(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature)
        {
            signer.Init(false, publicKey);
            signer.BlockUpdate(data);
            return signer.VerifySignature(signature.ToArray());
        }

        public void Dispose()
        {
        }
    }
}
