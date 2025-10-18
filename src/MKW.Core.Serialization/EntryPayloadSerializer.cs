// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Asn1;

namespace MKW.Core.Serialization
{
    public static class EntryPayloadSerializer
    {
        public static EntryPayload Deserialize(ReadOnlySpan<byte> data)
        {
            using Asn1InputStream stream = new Asn1InputStream(data.ToArray());
            {
                try
                {
                    Asn1Object obj = stream.ReadObject();
                    EntryPayloadStructure structure = new EntryPayloadStructure(Asn1Sequence.GetInstance(obj));
                    return structure.GetValue();
                }
                catch (Exceptions.InvalidEntryPayload)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new Exceptions.InvalidEntryPayload(ex);
                }
            }
        }

        public static ReadOnlyMemory<byte> Serialize(EntryPayload payload)
        {
            using MemoryStream stream = new MemoryStream();

            using Asn1OutputStream asn1 = Asn1OutputStream.Create(stream);
            {
                asn1.WriteObject(new EntryPayloadStructure(payload));
            }

            return stream.ToArray();
        }
    }
}
