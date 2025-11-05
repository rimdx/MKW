// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Cryptography.Exceptions;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace MKW.Cryptography.BouncyCastle
{
    public class RsaAsymmetricTransformer
        : IAsymmetricPrivateTransformer
        , IAsymmetricPublicTransformer
        , IDisposable
    {
        private readonly IBufferedCipher cipher;
        private readonly ISigner signer;

        private readonly AsymmetricPublicKey publicKey;
        private readonly AsymmetricPrivateKey? privateKey;

        public RsaAsymmetricTransformer(AsymmetricPublicKey publicKey,
                                        AsymmetricPrivateKey? privateKey,
                                        AsymmetricAlgorithmConfiguration config)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;

            cipher = CipherUtilities.GetCipher(PkcsObjectIdentifiers.RsaEncryption);

            IDigest digest = config.HashEngine.Visit(new HashAlgorithmDigestFactoryVisitor());
            signer = new RsaDigestSigner(digest);
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            try
            {
                cipher.Init(true, publicKey.GetParameter());

                using MemoryStream output = new MemoryStream();

                using (CipherStream cipherStream = new CipherStream(new StreamDisown(output),
                                                                    null, cipher))
                {
                    cipherStream.Write(data);
                }

                return output.ToArray();
            }
            catch (CryptoException ex)
            {
                throw new AsymmetricOperationFailedException(ex);
            }
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            if (privateKey == null)
            {
                throw new AsymmetricOperationRequiresPrivateKey();
            }

            try
            {
                cipher.Init(false, privateKey.GetParameter());

                using MemoryStream output = new MemoryStream();

                using (CipherStream cipherStream = new CipherStream(new StreamDisown(output),
                                                                    null, cipher))
                {
                    cipherStream.Write(data);
                }

                return output.ToArray();
            }
            catch (CryptoException ex)
            {
                throw new AsymmetricOperationFailedException(ex);
            }
        }

        public Memory<byte> ExportPrivateKey()
        {
            PrivateKeyInfo info = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey.GetParameter());
            return info.GetEncoded();
        }

        public Memory<byte> ExportPublicKey()
        {
            SubjectPublicKeyInfo info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey.GetParameter());
            return info.GetEncoded();
        }

        public Memory<byte> Sign(ReadOnlySpan<byte> data)
        {
            if (privateKey == null)
            {
                throw new AsymmetricOperationRequiresPrivateKey();
            }

            signer.Init(true, privateKey.GetParameter());
            signer.BlockUpdate(data);
            return signer.GenerateSignature();
        }

        public bool Verify(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature)
        {
            signer.Init(false, publicKey.GetParameter());
            signer.BlockUpdate(data);
            return signer.VerifySignature(signature.ToArray());
        }

        public void Dispose()
        {
        }
    }
}
