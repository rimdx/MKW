// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Exceptions;
using MKW.Core.Serialization;
using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    public class UserMetadataDecoder
    {
        private readonly IDatabase database;
        private readonly IAsymmetricPublicTransformer adminKey;

        public UserMetadataDecoder(IDatabase database, IAsymmetricPublicTransformer adminKey)
        {
            this.database = database;
            this.adminKey = adminKey;
        }

        public bool VerifyMetadata(DatabaseUserProtectedDataSigned protectedData)
        {
            ReadOnlyMemory<byte> bytes = database.SerializeProtectedData(protectedData);
            return adminKey.Verify(bytes.Span, protectedData.Signature.Span);
        }

        public UserMetadata OpenMetadata(DatabaseUser user)
        {
            if (VerifyMetadata(user.ProtectedData))
            {
                return UserMetadataSerializer.Deserialize(user.ProtectedData.Metadata.Span);
            }
            else
            {
                throw new InvalidUserMetadataSignature();
            }
        }

        public void Dispose()
        {
        }
    }
}
