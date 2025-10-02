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

            Asn1Sequence sequence = Asn1Sequence.GetInstance(stream.ReadObject());

            if (sequence.Count == 1)
            {
                Asn1Sequence innerSequence = Asn1Sequence.GetInstance(sequence[0]);
                UserAccessRequestStructure structure = new UserAccessRequestStructure(innerSequence);
                return structure.GetValue();
            }
            else
            {
                throw new ArgumentException("Bad sequence size: " + sequence.Count);
            }
        }
    }
}
