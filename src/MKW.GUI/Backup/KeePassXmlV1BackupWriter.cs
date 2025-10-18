// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Core.Serialization;
using MKW.Core.Serialization.KeePassXmlV1;
using MKW.GUI.Model;
using System.IO;

namespace MKW.GUI.Backup
{
    public sealed class KeePassXmlV1BackupWriter : IBackupWriter, IDisposable
    {
        private readonly Stream stream;
        private readonly KeePassXmlV1Writer writer;

        public KeePassXmlV1BackupWriter(Stream stream)
        {
            this.stream = stream;
            writer = new KeePassXmlV1Writer(stream);
        }

        public void WriteEntry(EntryPayload payload)
        {
            Dictionary<string, string> fields = [];

            foreach (KeyValuePair<EntryPayloadKey, string> property in payload)
            {
                string propname = ConvertPropertyName(property.Key);
                fields.Add(propname, property.Value);
            }

            BackupEntry backupEntry = new BackupEntry(fields);

            writer.WriteEntry(backupEntry);
        }

        private static string ConvertPropertyName(EntryPayloadKey key)
        {
            if (key == CommonEntryPropertiesModel.Title.Key)
            {
                return KeePassXmlV1CommonFields.Title;
            }
            else if (key == CommonEntryPropertiesModel.Username.Key)
            {
                return KeePassXmlV1CommonFields.Username;
            }
            else if (key == CommonEntryPropertiesModel.Password.Key)
            {
                return KeePassXmlV1CommonFields.Password;
            }
            else if (key == CommonEntryPropertiesModel.Url.Key)
            {
                return KeePassXmlV1CommonFields.Url;
            }
            else if (key == CommonEntryPropertiesModel.Notes.Key)
            {
                return KeePassXmlV1CommonFields.Notes;
            }
            else
            {
                return EntryPayloadKey.RelativeName(CommonEntryPropertiesModel.CustomPropertyNamespace, key);
            }
        }

        public void Dispose()
        {
            writer.Dispose();
            stream.Dispose();
        }
    }
}
