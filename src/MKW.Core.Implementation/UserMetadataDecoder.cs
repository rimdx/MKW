// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Exceptions;
using MKW.Core.Serialization;
using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Implementation
{
    public class UserMetadataDecoder
    {
        private readonly IAsymmetricPublicTransformer adminKey;

        public UserMetadataDecoder(IAsymmetricPublicTransformer adminKey)
        {
            this.adminKey = adminKey;
        }

        public bool VerifyMetadata(SignedPayload metadata)
        {
            return adminKey.Verify(metadata.Payload.Span, metadata.Signature.Span);
        }

        public UserMetadata OpenMetadata(DatabaseUser user)
        {
            if (VerifyMetadata(user.Metadata))
            {
                return UserMetadataSerializer.Deserialize(user.Metadata.Payload.Span);
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
