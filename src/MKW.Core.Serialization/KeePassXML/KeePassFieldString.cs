using System.Xml;
using System.Xml.Serialization;

namespace MKW.Core.Serialization.KeePassXML
{
    public sealed record class KeePassFieldString
    {
        [XmlElement("Key")]
        public required string Key { get; init; }

        [XmlElement("Value")]
        public required string Value { get; init; }
    }
}
