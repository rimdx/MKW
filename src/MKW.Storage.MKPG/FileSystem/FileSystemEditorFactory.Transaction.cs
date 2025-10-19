// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Storage.MKPG.FileSystem
{
    internal sealed partial class FileSystemEditorFactory
    {
        private sealed class Transaction : IFileEditorFactory.ITransaction
        {
            private readonly Stream oldFile;
            private readonly TempFile newFile;

            public Stream Writer => new StreamDisown(newFile);

            public Stream Reader => oldFile;

            public Transaction(FileSystemEditorFactory editor, Stream oldFile)
            {
                this.oldFile = oldFile;
                newFile = TempFile.Create(editor.Path, true);
            }

            public void Commit()
            {
                oldFile.Dispose();
                newFile.Accept();
            }

            public void Dispose()
            {
                oldFile.Dispose();
                newFile.Dispose();
            }
        }
    }
}
