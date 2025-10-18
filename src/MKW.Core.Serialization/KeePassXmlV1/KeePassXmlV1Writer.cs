// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Xml;

namespace MKW.Core.Serialization.KeePassXmlV1
{
    public sealed class KeePassXmlV1Writer : IDisposable
    {
        private readonly XmlWriter writer;
        private bool disposed = false;

        public KeePassXmlV1Writer(Stream stream)
        {
            writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Indent = true,
            });

            writer.WriteStartDocument();
            writer.WriteStartElement("pwlist");
        }

        public void WriteEntry(BackupEntry entry)
        {
            writer.WriteStartElement("pwentry");

            foreach (KeyValuePair<string, string> field in entry.Fields)
            {
                if (KeePassXmlV1CommonFields.KeywordFields.Contains(field.Key))
                {
                    writer.WriteStartElement(field.Key);
                }
                else
                {
                    writer.WriteStartElement("pwcustom");
                    writer.WriteAttributeString("key", field.Key);
                }

                writer.WriteString(field.Value);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                try
                {
                    writer.WriteEndDocument();
                }
                catch
                {
                }

                writer.Dispose();
                disposed = true;
            }
        }
    }
}
