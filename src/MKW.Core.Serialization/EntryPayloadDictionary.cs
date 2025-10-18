// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization.Exceptions;
using Org.BouncyCastle.Asn1;

namespace MKW.Core.Serialization
{
    internal sealed class EntryPayloadDictionary
        : Asn1Encodable
    {
        private readonly IReadOnlyCollection<KeyValuePair<EntryPayloadKey, string>> items;

        public EntryPayloadDictionary(IReadOnlyCollection<KeyValuePair<EntryPayloadKey, string>> items)
        {
            this.items = items;
        }

        public EntryPayloadDictionary(Asn1Set set)
        {
            Dictionary<EntryPayloadKey, string> items = [];

            foreach (Asn1Encodable? item in set)
            {
                Asn1Sequence sequence = Asn1Sequence.GetInstance(item);
                EntryPayloadKeyValuePair serialized = new EntryPayloadKeyValuePair(sequence);

                if (items.ContainsKey(serialized.Key))
                {
                    throw new InvalidEntryPayloadDuplicatedKeyException(serialized.Key);
                }

                items.Add(serialized.Key, serialized.Value);
            }

            this.items = items;
        }

        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector vector = new Asn1EncodableVector(items.Count);

            foreach (KeyValuePair<EntryPayloadKey, string> item in items)
            {
                vector.Add(new EntryPayloadKeyValuePair(item.Key, item.Value));
            }

            return new DerSet(vector);
        }

        public IReadOnlyCollection<KeyValuePair<EntryPayloadKey, string>> GetValue()
        {
            return items;
        }
    }
}
