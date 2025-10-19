// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Storage.MKPG
{
    internal sealed partial class FileSystemEditorFactory
    {
        private sealed class Transaction : IFileEditorTransaction
        {
            private readonly Stream oldFile;
            private readonly TempFile newFile;

            public Stream Writer => new StreamDisown(newFile);

            public Stream Reader => oldFile;

            public Transaction(FileSystemEditorFactory editor, Stream oldFile)
            {
                this.oldFile = oldFile;
                newFile = TempFile.Create(editor.Path);
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
