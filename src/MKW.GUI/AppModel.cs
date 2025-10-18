// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;
using MKW.Cryptography.Loader;
using MKW.GUI.Model;

namespace MKW.GUI
{
    public class AppModel
    {
        private readonly DocumentTable documentTable;
        private readonly ICryptographyProvider cryptographyProvider;

        public AppModel()
        {
            cryptographyProvider = CryptographyLoader.GetProvider();
            documentTable = new DocumentTable();
        }

        public IDocumentLock CreateDatabase(string databasePath, string password)
        {
            return documentTable.OpenDocument(databasePath, () => DatabaseModel.Create(cryptographyProvider, databasePath, password));
        }

        public IDocumentLock OpenDatabase(string filename)
        {
            return documentTable.OpenDocument(filename, () => DatabaseModel.Open(cryptographyProvider, filename));
        }
    }
}
