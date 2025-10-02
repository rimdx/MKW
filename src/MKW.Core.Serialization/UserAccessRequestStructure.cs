using Org.BouncyCastle.Asn1;

namespace MKW.Core.Serialization
{
    internal sealed class UserAccessRequestStructure
        : Asn1Encodable
    {
        private const int Version = 2;

        private readonly UserAccessRequest data;

        public UserAccessRequestStructure(UserAccessRequest data)
        {
            this.data = data;
        }

        public UserAccessRequestStructure(Asn1Sequence sequence)
        {
            using Asn1SequenceReader reader = Asn1SequenceReader.GetInstance(sequence);

            int version = DerInteger.GetInstance(reader.Next()).IntValueExact;

            if (version != Version)
            {
                throw new Exception($"Invalid version (expected {Version} but was {version}).");
            }

            data = new UserAccessRequest
            {
                Salt = Asn1OctetString.GetInstance(reader.Next()).GetOctets(),
                PublicKey = Asn1OctetString.GetInstance(reader.Next()).GetOctets(),
                EncryptedPrivateKey = new SecretPayload(Asn1OctetString.GetInstance(reader.Next()).GetOctets()),
                AdminSignature = Asn1OctetString.GetInstance(reader.Next()).GetOctets(),
            };
        }

        public override Asn1Object ToAsn1Object()
        {
            return new DerSequence(
                new DerInteger(Version),
                new DerOctetString(data.Salt.ToArray()),
                new DerOctetString(data.PublicKey.ToArray()),
                new DerOctetString(data.EncryptedPrivateKey.EncryptedPayload.ToArray()),
                new DerOctetString(data.AdminSignature.ToArray())
            );
        }

        public UserAccessRequest GetValue()
        {
            return data;
        }
    }
}
