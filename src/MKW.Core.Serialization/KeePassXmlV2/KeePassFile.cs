using System.Xml;
using System.Xml.Serialization;

namespace MKW.Core.Serialization.KeePassXmlV2
{
    [XmlRoot("KeePassFile")]
    public sealed record class KeePassFile
    {
        [XmlElement("Meta")]
        public required KeePassMeta Meta { get; init; }

        [XmlElement("Root")]
        public required KeePassRoot Root { get; init; }
    }
}
