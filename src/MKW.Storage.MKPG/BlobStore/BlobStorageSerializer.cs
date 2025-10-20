// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization.OpenPgp;

namespace MKW.Storage.MKPG.BlobStore
{
    internal static class BlobStorageSerializer
    {
        public static IEnumerable<PgpBlobEntry> ReadBlobs(TextReader reader)
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

                    yield return new PgpBlobEntry
                    {
                        Id = blobId,
                        Type = message.MessageTypeHeader,
                        Data = message.Data
                    };
                }
            }
        }

        public static void WriteBlobs(TextWriter writer, IEnumerable<PgpBlobEntry> blobs)
        {
            foreach (PgpBlobEntry blob in blobs)
            {
                WriteBlob(writer, blob);
            }
        }

        public static void WriteBlob(TextWriter writer, PgpBlobEntry blob)
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
