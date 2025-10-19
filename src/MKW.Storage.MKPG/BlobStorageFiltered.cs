// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG
{
    internal sealed class BlobStorageFiltered : IBlobStorage
    {
        private readonly IBlobStorage proxy;
        private readonly string type;

        public BlobStorageFiltered(IBlobStorage proxy, string type)
        {
            this.proxy = proxy;
            this.type = type;
        }

        public void Create(BlobEntry entry)
        {
            if (entry.Type != type)
            {
                throw new Exception("Attempted to created entry with wrong type.");
            }

            proxy.Create(entry);
        }

        public bool Delete(BlobId id)
        {
            // TODO: fix collision between different types with the same ids.
            return proxy.Delete(id);
        }

        public IEnumerable<BlobEntry> Enumerate()
        {
            foreach (BlobEntry entry in proxy.Enumerate())
            {
                if (entry.Type == type)
                {
                    yield return entry;
                }
            }
        }

        public bool Exists(BlobId id)
        {
            // TODO: fix collision between different types with the same ids.
            return proxy.Exists(id);
        }

        public BlobEntry Open(BlobId id)
        {
            // TODO: fix collision between different types with the same ids.
            return proxy.Open(id);
        }
    }
}
