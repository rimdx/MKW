// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public partial interface IDatabaseBlobStore
    {
        public interface ISnapshot
        {
            IEnumerable<Blob> Enumerate();
        }
    }
}
