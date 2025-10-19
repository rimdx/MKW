// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage.JSON
{
    internal sealed record class JSONDatabaseUser
    {
        public required ReadOnlyMemory<byte> Salt { get; init; }

        public required ReadOnlyMemory<byte> PublicKey { get; init; }

        public required ReadOnlyMemory<byte> PrivateKey { get; init; }

        public required ReadOnlyMemory<byte> Metadata { get; init; }
        public required ReadOnlyMemory<byte> MetadataAdminSignature { get; init; }

        public required ReadOnlyMemory<byte> AdminTrustSignature { get; init; }
        public required ReadOnlyMemory<byte> AdminSignature { get; init; }

        public static JSONDatabaseUser Serialize(DatabaseUser user)
        {
            return new JSONDatabaseUser
            {
                Salt = user.Salt,
                PrivateKey = user.PrivateKey.EncryptedPayload,

                PublicKey = user.PublicKey.Payload,
                AdminTrustSignature = user.PublicKey.Signature,

                Metadata = user.Metadata.Payload,
                MetadataAdminSignature = user.Metadata.Signature,

                AdminSignature = user.AdminSignature.SignatureBytes,
            };
        }

        public static DatabaseUser Deserialize(UserId id, JSONDatabaseUser user)
        {
            return new DatabaseUser
            {
                Id = id,
                Salt = user.Salt,
                PublicKey = new SignedPayload(user.PublicKey, user.AdminTrustSignature),
                PrivateKey = new SecretPayload(user.PrivateKey),
                Metadata = new SignedPayload(user.Metadata, user.MetadataAdminSignature),
                AdminSignature = new DatabaseTrustSignature
                {
                    Id = id,
                    SignatureBytes = user.AdminSignature,
                },
            };
        }
    }
}
