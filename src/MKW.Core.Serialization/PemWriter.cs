using Org.BouncyCastle.Asn1;

namespace MKW.Core.Serialization
{
    internal sealed class PemWriter
        : IDisposable
    {
        private readonly TextWriter writer;

        private readonly string header;
        private readonly string footer;

        public PemWriter(TextWriter writer, string type)
        {
            this.writer = writer;

            header = $"-----BEGIN {type}-----";
            footer = $"-----END {type}-----";
        }

        public void AddObject(Asn1Encodable obj)
        {
            writer.WriteLine(header);

            using (MemoryStream stream = new MemoryStream())
            {
                obj.EncodeTo(stream, Asn1Encodable.Der);

                byte[] bytes = stream.ToArray();
                string base64 = Convert.ToBase64String(bytes, Base64FormattingOptions.InsertLineBreaks);

                writer.WriteLine(base64);
            }

            writer.WriteLine(footer);
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
