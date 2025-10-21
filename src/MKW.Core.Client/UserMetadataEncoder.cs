// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class UserMetadataEncoder
    {
        private readonly IAsymmetricPrivateTransformer adminKey;

        public UserMetadataEncoder(IAsymmetricPrivateTransformer adminKey)
        {
            this.adminKey = adminKey;
        }

        public SignedPayload EncodeMetadata(UserMetadata metadata)
        {
            ReadOnlyMemory<byte> encoded = UserMetadataSerializer.Serialize(metadata);
            ReadOnlyMemory<byte> signature = adminKey.Sign(encoded.Span);

            return new SignedPayload(encoded, signature);
        }
    }
}
