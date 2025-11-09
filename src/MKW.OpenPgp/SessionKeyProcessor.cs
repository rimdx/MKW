// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization.OpenPgp.Packets;
using MKW.Cryptography;
using Org.BouncyCastle.Bcpg;

namespace MKW.OpenPgp
{
    internal sealed class SessionKeyProcessor
    {
        private readonly ICryptographyProvider crypto;

        public SessionKeyProcessor(ICryptographyProvider crypto)
        {
            this.crypto = crypto;
        }

        public SymmetricKey OpenKey(SessionKeyPayload payload)
        {
            ushort checksum = GetChecksum(payload.KeyBytes.Span);

            if (checksum != payload.Checksum)
            {
                throw new SessionKeyChecksumMismatchException(checksum, payload.Checksum);
            }

            if (payload.SymmetricAlgorithm == SymmetricKeyAlgorithmTag.Aes256)
            {
                return crypto.OpenSymmetricKey(CommonCryptographyAlgorithms.Aes256OpenPgpCfb,
                                               payload.KeyBytes,
                                               null);
            }
            else if (payload.SymmetricAlgorithm == SymmetricKeyAlgorithmTag.Aes128)
            {
                return crypto.OpenSymmetricKey(CommonCryptographyAlgorithms.Aes128OpenPgpCfb,
                                               payload.KeyBytes,
                                               null);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        // Then a two-octet checksum is appended, which is equal to the
        // sum of the preceding session key octets, not including the algorithm
        // identifier, modulo 65536.
        private static ushort GetChecksum(ReadOnlySpan<byte> data)
        {
            ushort checksum = 0;

            foreach (byte octet in data)
            {
                checksum += octet;
            }

            // #letsbeextrasafe
            return (ushort)(checksum % 65536);
        }
    }
}
