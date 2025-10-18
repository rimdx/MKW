// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

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
