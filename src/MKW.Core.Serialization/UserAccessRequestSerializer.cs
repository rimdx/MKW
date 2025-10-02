using Org.BouncyCastle.Asn1;

namespace MKW.Core.Serialization
{
    public static class UserAccessRequestSerializer
    {
        public static string Serialize(UserAccessRequest data)
        {
            using MemoryStream stream = new MemoryStream();

            using (DerSequenceGenerator writer = new DerSequenceGenerator(stream))
            {
                writer.AddObject(new UserAccessRequestStructure(data));
            }

            byte[] bytes = stream.ToArray();

            return Convert.ToBase64String(bytes);
        }

        public static UserAccessRequest Deserialize(string data)
        {
            byte[] bytes = Convert.FromBase64String(data);

            using Asn1InputStream stream = new Asn1InputStream(bytes);

            using (Asn1SequenceReader sequence = Asn1SequenceReader.GetInstance(stream.ReadObject()))
            {
                Asn1Sequence innerSequence = Asn1Sequence.GetInstance(sequence.Next());
                UserAccessRequestStructure structure = new UserAccessRequestStructure(innerSequence);
                return structure.GetValue();
            }
        }
    }
}
