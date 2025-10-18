// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG
{
    internal static class BlobIdSerializer
    {
        public static string Serialize(BlobId blobId)
        {
            Guid guid = new Guid(blobId.GetBytes().ToArray());
            return guid.ToString();
        }

        public static BlobId Deserialize(string str)
        {
            Guid guid = new Guid(str);
            return BlobId.From(guid);
        }
    }
}
