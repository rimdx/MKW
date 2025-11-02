// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG.FileSystem
{
    public sealed partial class MemoryEditorFactory : IFileEditorFactory, IDisposable
    {
        public MemoryStream Buffer { get; private set; }

        public MemoryEditorFactory()
        {
            Buffer = new MemoryStream();
        }

        public Stream CreateReader()
        {
            byte[] snapshot = Buffer.ToArray();
            MemoryStream stream = new MemoryStream(snapshot);
            return stream;
        }

        public IFileEditorFactory.ITransaction CreateTransaction()
        {
            return new Transaction(this, CreateReader());
        }

        public byte[] ToArray()
        {
            return Buffer.ToArray();
        }

        public void Dispose()
        {
            Buffer.Dispose();
        }
    }
}
