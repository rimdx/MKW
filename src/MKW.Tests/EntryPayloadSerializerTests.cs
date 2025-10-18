// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Core.Serialization;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class EntryPayloadSerializerTests
    {
        [Test]
        public void SimpleTest()
        {
            using ClientSandBox sbox = new ClientSandBox();

            EntryPayload p1 = new EntryPayload();
            p1.SetProperty(new EntryPayloadKey("mkw:username"), "rinrab");
            p1.SetProperty(new EntryPayloadKey("mkw:password"), "hah. no");
            p1.SetProperty(new EntryPayloadKey("mkw:password"), "hah. no2");

            Console.WriteLine(p1);

            ReadOnlyMemory<byte> data = EntryPayloadSerializer.Serialize(p1);

            Console.WriteLine(Convert.ToBase64String(data.ToArray()));

            EntryPayload p2 = EntryPayloadSerializer.Deserialize(data.Span);

            ClassicAssert.AreEqual(p1, p2);
        }
    }
}
