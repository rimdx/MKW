using System.Buffers;

namespace MKW.Core.Serialization.Pgp.Primitives
{
    internal sealed class PgpVersion
    {
        private readonly int expectedVersion;

        public PgpVersion(int expectedVersion)
        {
            this.expectedVersion = expectedVersion;
        }

        public void ConsumeVersion(ArrayBufferReader reader)
        {
            int version = reader.ReadByte();

            if (version != expectedVersion)
            {
                throw new PgpVersionMismatchException(expectedVersion, version);
            }
            else
            {
                /* we're fine */
            }
        }

        public void Serialize(IBufferWriter<byte> writer)
        {
            writer.Write((byte)expectedVersion);
        }
    }
}
