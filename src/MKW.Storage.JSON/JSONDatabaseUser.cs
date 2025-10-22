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

                PublicKey = user.ProtectedData.PublicKey,
                AdminTrustSignature = user.ProtectedData.Signature,

                Metadata = user.ProtectedData.Metadata,
                MetadataAdminSignature = null,

                AdminSignature = null,
            };
        }

        public static DatabaseUser Deserialize(UserId id, JSONDatabaseUser user)
        {
            return new DatabaseUser
            {
                Id = id,
                Salt = user.Salt,
                ProtectedData = new DatabaseUserProtectedDataSigned
                {
                    PublicKey = user.PublicKey,
                    Metadata = user.Metadata,
                    Signature = user.AdminTrustSignature,
                },
                PrivateKey = new SecretPayload(user.PrivateKey),
            };
        }
    }
}
