// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Asn1;

namespace MKW.Core.Serialization
{
    internal sealed class EntryPayloadKeyValuePair
        : Asn1Encodable
    {
        public EntryPayloadKey Key { get; }
        public string Value { get; }

        public EntryPayloadKeyValuePair(Asn1Sequence sequence)
        {
            using Asn1SequenceReader reader = new Asn1SequenceReader(sequence);

            Key = new EntryPayloadKey(DerUtf8String.GetInstance(reader.Next()).GetString());
            Value = DerUtf8String.GetInstance(reader.Next()).GetString();
        }

        public EntryPayloadKeyValuePair(EntryPayloadKey key, string value)
        {
            Key = key;
            Value = value;
        }

        public override Asn1Object ToAsn1Object()
        {
            return new DerSequence(
                new DerUtf8String(Key.ToString()),
                new DerUtf8String(Value)
            );
        }
    }
}
