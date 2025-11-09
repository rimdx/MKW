// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core.Serialization.OpenPgp;
using MKW.Core.Serialization.OpenPgp.Packets;
using MKW.Cryptography;
using Org.BouncyCastle.Bcpg;

namespace MKW.OpenPgp
{
    public sealed class PgpPrivateKey : IDisposable
    {
        private readonly List<IAsymmetricPrivateTransformer> keys;
        private readonly ICryptographyProvider crypto;

        public PgpPrivateKey(ICryptographyProvider crypto)
        {
            keys = [];
            this.crypto = crypto;
        }

        public void Import(SecretKeyPacketV4 seckeyPacket)
        {
            PublicKeyMaterialRSA pubkeyMaterial = (PublicKeyMaterialRSA)seckeyPacket.PublicKey.PublicKeyMaterial;

            //StringToKeySaltedIterated s2k = (StringToKeySaltedIterated)seckeyPacket.StringToKey.StringToKey;
            // ReadOnlyMemory<byte> salt = s2k.Salt;

            ReadOnlyMemory<byte> seckey = seckeyPacket.SecretKeyData;
            SecretKeyMaterialRSA seckeyMaterial = SecretKeyMaterialRSASerializer.Deserialize(new ArrayBufferReader<byte>(seckey));

            AsymmetricPrivateKey key = PrivateKeyConverter.OpenPrivateKey(pubkeyMaterial, seckeyMaterial);
            IAsymmetricPrivateTransformer transformer = crypto.OpenAsymmetricTransformer(key, CommonCryptographyAlgorithms.Rsa2048);
            keys.Add(transformer);
        }

        public void Import(PgpArmouredMessage msg)
        {
            IBufferReader<byte> reader = msg.CreateReader();

            while (reader.RemainingBytes > 0)
            {
                PgpPacket packet = PgpPacketSerializer.ReadPacket(reader);
                IBufferReader<byte> subreader = packet.CreateReader();

                // A Secret-Subkey packet (tag 7) is the subkey analog of the Secret
                // Key packet and has exactly the same format.
                if (packet.Tag == PacketTag.SecretKey || packet.Tag == PacketTag.SecretSubkey)
                {
                    Import(SecretKeyPacketV4Serializer.Deserialize(subreader));
                }
            }
        }

        public ReadOnlyMemory<byte> DecryptMessage(PgpMessage message)
        {
            List<Exception> errors = [];

            foreach (IAsymmetricPrivateTransformer key in keys)
            {
                try
                {
                    return message.Decrypt(crypto, key);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }

            throw new Exception("No valid private key was found.", new AggregateException(errors));
        }

        public void Dispose()
        {
        }
    }
}
