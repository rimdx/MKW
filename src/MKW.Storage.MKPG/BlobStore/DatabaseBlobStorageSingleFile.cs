// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Storage.MKPG.FileSystem;

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed partial class DatabaseBlobStorageSingleFile : IDatabaseBlobStore
    {
        private readonly IFileEditorFactory editor;

        public DatabaseBlobStorageSingleFile(IFileEditorFactory editor)
        {
            this.editor = editor;
        }

        public IDatabaseBlobStore.ITransaction BeginTransaction()
        {
            return new Transaction(editor.CreateTransaction());
        }

        public IDatabaseBlobStore.ISnapshot CreateSnapshot()
        {
            return new Snapshot(editor.CreateReader());
        }

        public void Dispose()
        {
            editor.Dispose();
        }
    }
}
