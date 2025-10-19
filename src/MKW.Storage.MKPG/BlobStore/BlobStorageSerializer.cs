// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization.OpenPgp;

namespace MKW.Storage.MKPG.BlobStore
{
    internal static class BlobStorageSerializer
    {
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
                    PgpArmourHeader id = message.GetHeader(MKPGConstants.ArmourHeaderNames.Id);
                    BlobId blobId = BlobIdSerializer.Deserialize(id.Value);

                    yield return new BlobEntry(blobId, message.MessageTypeHeader, message.Data);
                }
            }
        }

        public static void WriteBlobs(TextWriter writer, IEnumerable<BlobEntry> blobs)
        {
            foreach (BlobEntry blob in blobs)
            {
                WriteBlob(writer, blob);
            }
        }

        public static void WriteBlob(TextWriter writer, BlobEntry blob)
        {
            PgpArmouredMessage msg = new PgpArmouredMessage
            {
                MessageTypeHeader = blob.Type,
                Headers = [
                    new PgpArmourHeader(MKPGConstants.ArmourHeaderNames.Id, BlobIdSerializer.Serialize(blob.Id)),
                ],
                Data = blob.Data,
            };

            PgpArmouredMessageSerializer.Serialize(writer, msg);
        }
    }
}
