using System.Xml;
using System.Xml.Serialization;

namespace MKW.Core.Serialization.KeePassXML
{
    public sealed record class KeePassEntry
    {
        [XmlElement("String")]
        public required KeePassFieldString[]? Fields { get; init; }
    }
}
