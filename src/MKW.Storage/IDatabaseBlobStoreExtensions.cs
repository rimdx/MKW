// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public static class IDatabaseBlobStoreExtensions
    {
        public static IEnumerable<T> EnumerateBlobEntry<T>(this IDatabaseBlobStore.ISnapshot snapshot) where T : Blob
        {
            foreach (Blob blob in snapshot.Enumerate())
            {
                if (blob is T entry)
                {
                    yield return entry;
                }
            }
        }
    }
}
