// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG
{
    internal sealed partial class MemoryEditorFactory
    {
        private sealed class Transaction : IFileEditorFactory.ITransaction, IDisposable
        {
            private readonly MemoryEditorFactory editor;
            private readonly MemoryStream stream;

            public Stream Writer => stream;

            public Stream Reader { get; }

            public Transaction(MemoryEditorFactory editor, Stream reader)
            {
                this.editor = editor;

                Reader = reader;
                stream = new MemoryStream();
            }

            public void Commit()
            {
                editor.Buffer = stream;
            }

            public void Dispose()
            {
                Writer.Dispose();
            }
        }
    }
}
