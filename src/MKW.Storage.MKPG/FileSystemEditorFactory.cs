// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG
{
    internal sealed partial class FileSystemEditorFactory : IFileEditorFactory
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

        public IFileEditorTransaction OpenTransaction()
        {
            return new Transaction(this, CreateReader());
        }

        public void Dispose()
        {
        }
    }
}
