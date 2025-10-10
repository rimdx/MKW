using System.Xml;

namespace MKW.Core.Serialization.KeePassXmlV1
{
    public sealed class KeePassXmlV1Writer : IDisposable
    {
        private readonly XmlWriter writer;

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
                writer.WriteStartElement(field.Key);
                writer.WriteString(field.Value);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public void Dispose()
        {
            writer.WriteEndDocument();
            writer.Dispose();
        }
    }
}
