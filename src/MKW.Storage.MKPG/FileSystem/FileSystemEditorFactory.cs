// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG.FileSystem
{
    public sealed partial class FileSystemEditorFactory : IFileEditorFactory
    {
        public string Path { get; }

        public FileSystemEditorFactory(string path)
        {
            Path = path;
        }

        public Stream CreateReader()
        {
            return File.OpenRead(Path);
        }

        public IFileEditorFactory.ITransaction CreateTransaction()
        {
            return new Transaction(this, CreateReader());
        }

        public void Dispose()
        {
        }
    }
}
