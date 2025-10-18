// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Xml;
using System.Xml.Serialization;

namespace MKW.Core.Serialization.KeePassXmlV2
{
    public sealed record class KeePassFieldString
    {
        [XmlElement("Key")]
        public required string Key { get; init; }

        [XmlElement("Value")]
        public required string Value { get; init; }
    }
}
