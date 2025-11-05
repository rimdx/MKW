// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Cryptography.Exceptions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Buffers;

namespace MKW.Cryptography.BouncyCastle
{
    internal sealed class AesOpenPgpTransformer : ISymmetricTransformer, IDisposable
    {
        private readonly SymmetricKeyAesOpenPgpCfb key;
        private readonly SymmetricCipher cipher;
        private readonly OpenPgpModificationDetectionPacketGenerator mdpGenerator;

        public AesOpenPgpTransformer(SymmetricKeyAesOpenPgpCfb key)
        {
            this.key = key;

            AesEngine blockCipher = new AesEngine();
            CfbBlockCipher blockCipherMode = new CfbBlockCipher(blockCipher, blockCipher.GetBlockSize());
            BufferedBlockCipher cipher = new BufferedBlockCipher(blockCipherMode);

            KeyParameter parameters = new KeyParameter(key.KeyBytes.ToArray());

            this.cipher = new SymmetricCipher(cipher, parameters);

            Sha1Digest digest = new Sha1Digest();
            SecureRandom random = new SecureRandom();
            mdpGenerator = new OpenPgpModificationDetectionPacketGenerator(digest, random);
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            ReadOnlyMemory<byte> raw = cipher.Decrypt(data);

            ArrayBufferReader<byte> reader = new ArrayBufferReader<byte>(raw);

            ReadOnlyMemory<byte> plaintext;
            try
            {
                OpenPgpModificationDetectionPacket packet =
                    OpenPgpModificationDetectionPacketSerializer.Deserialize(reader);

                plaintext = mdpGenerator.OpenPlaintext(packet);
            }
            catch (OpenPgpModificationDetectionPacketCorruptedException ex)
            {
                throw new SymmetricOperationFailedException(ex);
            }

            return plaintext.ToArray(); // todo!!
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();

            try
            {
                OpenPgpModificationDetectionPacket packet = mdpGenerator.CreatePacket(data);
                OpenPgpModificationDetectionPacketSerializer.Serialize(writer, packet);
            }
            catch (OpenPgpModificationDetectionPacketCorruptedException ex)
            {
                throw new SymmetricOperationFailedException(ex);
            }

            return cipher.Encrypt(writer.WrittenSpan);
        }

        public void Dispose()
        {
            // no-op
        }
    }
}
