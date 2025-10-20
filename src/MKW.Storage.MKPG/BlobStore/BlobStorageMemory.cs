// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Storage.Exceptions;

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed class BlobStorageMemory : IBlobStorage
    {
        private readonly Dictionary<BlobId, PgpBlobEntry> entries;

        public BlobStorageMemory()
        {
            entries = [];
        }

        public void Create(PgpBlobEntry entry)
        {
            if (entries.ContainsKey(entry.Id))
            {
                throw new EntryAlreadyExistsException();
            }

            entries[entry.Id] = entry;
        }

        public void Update(PgpBlobEntry entry)
        {
            entries[entry.Id] = entry;
        }

        public PgpBlobEntry Open(BlobId id)
        {
            return entries[id];
        }

        public bool Delete(BlobId id)
        {
            return entries.Remove(id);
        }

        public bool Exists(BlobId id)
        {
            return entries.ContainsKey(id);
        }

        public IEnumerable<PgpBlobEntry> Enumerate()
        {
            return entries.Values;
        }
    }
}
