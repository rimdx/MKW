// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Asn1;

namespace MKW.Core.Serialization
{
    internal sealed class EntryPayloadStructure
        : Asn1Encodable
    {
        private static readonly Asn1Version version = new Asn1Version(2);

        private readonly EntryPayload data;

        public EntryPayloadStructure(EntryPayload data)
        {
            this.data = data;
        }

        public EntryPayloadStructure(Asn1Sequence sequence)
        {
            using Asn1SequenceReader reader = new Asn1SequenceReader(sequence);

            version.ConsumeVersion(reader.Next());

            Asn1Set set = Asn1Set.GetInstance(reader.Next());
            EntryPayloadDictionary items = new EntryPayloadDictionary(set);

            data = EntryPayload.FromDictionary(items.GetValue());
        }

        public override Asn1Object ToAsn1Object()
        {
            return new DerSequence(
                version,
                new EntryPayloadDictionary(data)
            );
        }

        public EntryPayload GetValue()
        {
            return data;
        }
    }
}
