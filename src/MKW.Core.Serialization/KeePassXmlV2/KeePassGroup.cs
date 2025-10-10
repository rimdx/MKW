using System.Xml;
using System.Xml.Serialization;

namespace MKW.Core.Serialization.KeePassXmlV2
{
    public sealed record class KeePassGroup
    {
        [XmlElement("Group")]
        public required KeePassGroup[]? Groups { get; init; }

        [XmlElement("Entry")]
        public required KeePassEntry[]? Entries { get; init; }
    }
}
