using System.Xml;

namespace MKW.Core.Serialization.KeePassXmlV1
{
    public sealed class KeePassXmlV1Reader : IDisposable
    {
        private readonly XmlReader reader;

        public KeePassXmlV1Reader(Stream stream)
        {
            reader = XmlReader.Create(stream);
        }

        public BackupEntry? ReadEntry()
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "pwentry")
                    {
                        Dictionary<string, string> fields = [];

                        using (XmlReader entryReader = reader.ReadSubtree())
                        {
                            foreach (KeyValuePair<string, string> pair in ReadEntryElement(entryReader))
                            {
                                fields.Add(pair.Key, pair.Value);
                            }
                        }

                        return new BackupEntry(fields);
                    }
                    else if (reader.Name == "pwlist")
                    {
                        // ingore root
                    }
                    else
                    {
                        throw new Exception($"Unexpected tag: {reader.Name}");
                    }
                }
            }

            return null;
        }

        private static IEnumerable<KeyValuePair<string, string>> ReadEntryElement(XmlReader entryReader)
        {
            entryReader.MoveToContent(); // <pwentry>

            while (entryReader.Read())
            {
                if (entryReader.NodeType == XmlNodeType.Element)
                {
                    using XmlReader fieldReader = entryReader.ReadSubtree();

                    yield return ReadEntryField(fieldReader);
                }
                else if (entryReader.NodeType == XmlNodeType.EndElement)
                {
                    yield break;
                }
            }
        }

        private static KeyValuePair<string, string> ReadEntryField(XmlReader fieldReader)
        {
            string? key = null;
            string? content = null;

            while (fieldReader.Read())
            {
                if (fieldReader.NodeType == XmlNodeType.Text)
                {
                    content = fieldReader.ReadContentAsString();
                }
                if (fieldReader.NodeType == XmlNodeType.Element)
                {
                    if (key == null)
                    {
                        if (fieldReader.Name == "pwcustom")
                        {
                            key = fieldReader.GetAttribute("key");
                        }
                        else
                        {
                            key = fieldReader.Name;
                        }
                    }
                    else
                    {
                        throw new Exception($"Unexpected tag: {fieldReader.Name}.");
                    }
                }
                else if (fieldReader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
            }

            if (key == null)
            {
                throw new Exception("Key is missing.");
            }

            return new KeyValuePair<string, string>(key, content ?? string.Empty);
        }

        public IEnumerable<BackupEntry> GetEntriesEnumerator()
        {
            while (true)
            {
                BackupEntry? entry = ReadEntry();

                if (entry != null)
                {
                    yield return entry;
                }
                else
                {
                    yield break;
                }
            }
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
