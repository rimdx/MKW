using System.Xml;
using System.Xml.Serialization;

namespace MKW.Core.Serialization.KeePassXmlV2
{
    public sealed record class KeePassEntry
    {
        [XmlElement("String")]
        public required KeePassFieldString[]? Fields { get; init; }
    }
}
