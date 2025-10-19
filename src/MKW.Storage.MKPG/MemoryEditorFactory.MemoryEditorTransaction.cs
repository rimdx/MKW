// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG
{
    internal sealed partial class MemoryEditorFactory
    {
        private sealed class MemoryEditorTransaction : IFileEditorTransaction, IDisposable
        {
            private readonly MemoryEditorFactory editor;
            private readonly MemoryStream stream;

            public Stream Stream => stream;

            public MemoryEditorTransaction(MemoryEditorFactory editor)
            {
                this.editor = editor;
                stream = new MemoryStream();
            }

            public void Commit()
            {
                editor.Buffer = stream;
            }

            public void Dispose()
            {
                Stream.Dispose();
            }
        }
    }
}
