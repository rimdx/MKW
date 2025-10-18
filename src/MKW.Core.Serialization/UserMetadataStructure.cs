// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Asn1;

namespace MKW.Core.Serialization
{
    internal sealed class UserMetadataStructure
        : Asn1Encodable
    {
        private static readonly Asn1Version version = new Asn1Version(2);

        private readonly UserMetadata data;

        public UserMetadataStructure(UserMetadata data)
        {
            this.data = data;
        }

        public UserMetadataStructure(Asn1Sequence sequence)
        {
            using Asn1SequenceReader reader = new Asn1SequenceReader(sequence);

            version.ConsumeVersion(reader.Next());

            data = new UserMetadata
            {
                UserId = ((DerUtf8String)reader.Next()).GetString(),
                DisplayName = ((DerUtf8String)reader.Next()).GetString(),
            };
        }

        public override Asn1Object ToAsn1Object()
        {
            return new DerSequence(
                version,
                new DerUtf8String(data.UserId),
                new DerUtf8String(data.DisplayName)
            );
        }

        public UserMetadata GetValue()
        {
            return data;
        }
    }
}
