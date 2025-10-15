namespace MKW.Core.Serialization.Pgp.Primitives
{
    internal sealed class PgpVersion : PgpObject
    {
        private readonly int expectedVersion;

        public PgpVersion(int expectedVersion)
        {
            this.expectedVersion = expectedVersion;
        }

        public void ConsumeVersion(PgpInputStream stream)
        {
            int version = stream.RequireByte();

            if (version != expectedVersion)
            {
                throw new PgpVersionMismatchException(expectedVersion, version);
            }
            else
            {
                /* we're fine */
            }
        }

        public override void Encode(PgpOutputStream stream)
        {
            stream.WriteByte((byte)expectedVersion);
        }
    }
}
