// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core;
using MKW.Core.Serialization.CSV;
using MKW.GUI.Model;
using System.IO;

namespace MKW.GUI.Backup
{
    public sealed class KeePassCSVBackup : IBackupReader, IDisposable
    {
        private readonly Stream file;

        private static readonly IReadOnlyList<EntryPayloadKey> FieldMap = [
            CommonEntryPropertiesModel.Title.Key,
            CommonEntryPropertiesModel.Username.Key,
            CommonEntryPropertiesModel.Password.Key,
            CommonEntryPropertiesModel.Url.Key,
            CommonEntryPropertiesModel.Notes.Key,
        ];

        public KeePassCSVBackup(Stream file)
        {
            this.file = file;
        }

        public IEnumerable<EntryPayload> EnumerateEntries()
        {
            file.Seek(0, SeekOrigin.Begin);

            using StreamReader reader = new StreamReader(new StreamDisown(file));
            using CSVTokenReader tokens = new CSVTokenReader(reader);
            using CSVSerializer csv = new CSVSerializer(tokens);

            CSVRow? header = csv.ReadRow();

            if (header == null)
            {
                throw new Exception("Header row is missing.");
            }

            if (header.Count != FieldMap.Count)
            {
                throw new Exception("Header length does not match expected field count.");
            }

            foreach (CSVRow row in csv.EnumerateRows())
            {
                if (row.Count != FieldMap.Count)
                {
                    throw new Exception("Row length does not match header length.");
                }

                EntryPayload entry = new EntryPayload();

                for (int i = 0; i < row.Count; i++)
                {
                    EntryPayloadKey key = FieldMap[i];
                    CSVField value = row[entry.Count];

                    entry.SetProperty(key, value.Value);
                }

                yield return entry;
            }
        }

        public void Dispose()
        {
            file.Dispose();
        }
    }
}
