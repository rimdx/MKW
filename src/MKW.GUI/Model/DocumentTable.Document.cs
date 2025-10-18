// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.GUI.Model
{
    public sealed partial class DocumentTable
    {
        private sealed partial class Document : IDisposable
        {
            public DatabaseModel Database { get; }
            private int lockCount;
            private bool disposed;
            private readonly DocumentTable documentTable;

            private Document(DocumentTable documentTable, DatabaseModel database)
            {
                this.documentTable = documentTable;
                Database = database;
                lockCount = 0;
            }

            public static Document Make(DocumentTable documentTable, DatabaseModel database)
            {
                return new Document(documentTable, database);
            }

            public IDocumentLock ObtainLock()
            {
                lockCount++;

                return new DocumentLock(this);
            }

            public void Dispose()
            {
                if (!disposed)
                {
                    Database.Dispose();
                    documentTable.documents.Remove(this);

                    disposed = true;
                }
            }

            private void ReleaseLock()
            {
                lockCount--;
                if (lockCount == 0)
                {
                    Dispose();
                }
            }
        }
    }
}
