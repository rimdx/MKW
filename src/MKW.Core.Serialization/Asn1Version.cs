using Org.BouncyCastle.Asn1;

namespace MKW.Core.Serialization
{
    internal sealed class Asn1Version
        : Asn1Encodable
    {
        private readonly int expectedVersion;

        public Asn1Version(int expectedVersion)
        {
            this.expectedVersion = expectedVersion;
        }

        public void ConsumeVersion(Asn1Encodable obj)
        {
            DerInteger integer = DerInteger.GetInstance(obj);
            int version = integer.IntValueExact;

            if (version != expectedVersion)
            {
                throw new Asn1VersionMismatch(expectedVersion, version);
            }
            else
            {
                /* we're fine */
            }
        }

        public override Asn1Object ToAsn1Object()
        {
            return new DerInteger(expectedVersion);
        }
    }
}
