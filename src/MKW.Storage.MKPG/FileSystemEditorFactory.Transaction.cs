// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Storage.MKPG
{
    internal sealed partial class FileSystemEditorFactory
    {
        private sealed class Transaction : IFileEditorTransaction
        {
            private readonly TempFile file;

            public Stream Stream => file;

            public Transaction(FileSystemEditorFactory editor)
            {
                file = TempFile.Create(editor.Path);
            }

            public void Commit()
            {
                file.Accept();
            }

            public void Dispose()
            {
                file.Dispose();
            }
        }
    }
}
