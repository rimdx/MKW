using Org.BouncyCastle.Asn1;

namespace MKW.Core.Serialization
{
    public static class UserAccessRequestSerializer
    {
        private const string Type = "MKW ACCESS REQUEST";

        public static string Serialize(UserAccessRequest data)
        {
            using StringWriter text = new StringWriter();
            using (PemWriter pem = new PemWriter(text, Type))
            {
                pem.AddObject(new UserAccessRequestStructure(data));
            }

            return text.ToString();
        }

        public static UserAccessRequest Deserialize(string data)
        {
            using StringReader text = new StringReader(data);
            using PemReader pem = new PemReader(text, Type);

            try
            {
                Asn1Sequence sequence = Asn1Sequence.GetInstance(pem.ReadObject());
                UserAccessRequestStructure structure = new UserAccessRequestStructure(sequence);
                return structure.GetValue();
            }
            catch (Exception ex)
            {
                throw new Exceptions.InvalidUserAccessRequestException(ex);
            }
        }
    }
}
