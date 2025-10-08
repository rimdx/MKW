using System.Xml;
using System.Xml.Serialization;

namespace MKW.Core.Serialization.KeePassXML
{
    public sealed record class KeePassRoot
    {
        [XmlElement("Group")]
        public required KeePassGroup RootGroup { get; init; }
    }
}
