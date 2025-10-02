using Org.BouncyCastle.Asn1;

namespace MKW.Core.Serialization
{
    public static class UserMetadataSerializer
    {
        public static UserMetadata Deserialize(ReadOnlySpan<byte> data)
        {
            using Asn1InputStream stream = new Asn1InputStream(data.ToArray());
            {
                Asn1Object obj = stream.ReadObject();
                UserMetadataStructure structure = new UserMetadataStructure(Asn1Sequence.GetInstance(obj));
                return structure.GetValue();
            }
        }

        public static ReadOnlyMemory<byte> Serialize(UserMetadata metadata)
        {
            using MemoryStream stream = new MemoryStream();

            using Asn1OutputStream asn1 = Asn1OutputStream.Create(stream);
            {
                asn1.WriteObject(new UserMetadataStructure(metadata));
            }

            return stream.ToArray();
        }
    }
}
