using MKW.Core.Serialization.Pgp;

namespace MKW.Storage.MKPG
{
    internal static class BlobStorageSerializer
    {
        private const string idHeader = "MKWID";

        public static IEnumerable<BlobEntry> ReadBlobs(TextReader reader)
        {
            while (true)
            {
                PgpArmouredMessage? message = PgpArmouredMessageSerializer.Deserialize(reader);

                if (message == null)
                {
                    yield break;
                }
                else
                {
                    PgpArmourHeader id = message.Headers.First(header => header.Key == idHeader);

                    yield return new BlobEntry(BlobId.From(new Guid(id.Value)), message.Data);
                }
            }
        }

        public static void WriteBlobs(TextWriter writer, IEnumerable<BlobEntry> blobs)
        {
            foreach (BlobEntry blob in blobs)
            {
                PgpArmouredMessage msg = new PgpArmouredMessage
                {
                    MessageTypeHeader = "MKW ENTRY",
                    Headers = [
                        new PgpArmourHeader(idHeader, new Guid(blob.Id.GetBytes().ToArray()).ToString()),
                    ],
                    Data = blob.Data,
                };

                PgpArmouredMessageSerializer.Serialize(writer, msg);
            }
        }
    }
}
