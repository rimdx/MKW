// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Core.Exceptions;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class EntryPayloadTests
    {
        [Test]
        public void EntryPayloadKeyParseTests()
        {
            EntryPayloadKey k1 = new EntryPayloadKey("a:b:c");
            ClassicAssert.AreEqual("a:b:c", k1.ToString());

            EntryPayloadKey branch = k1.Branch("x");

            Assert.Throws<InvalidEntryPayloadKey>(() => new EntryPayloadKey("a:b:c:"));
            Assert.Throws<InvalidEntryPayloadKey>(() => new EntryPayloadKey(""));
            Assert.Throws<InvalidEntryPayloadKey>(() => new EntryPayloadKey("ъъ"));
            Assert.Throws<InvalidEntryPayloadKey>(() => branch.Branch(""));
            Assert.Throws<InvalidEntryPayloadKey>(() => branch.Branch("ъъ"));
            Assert.Throws<InvalidEntryPayloadKey>(() => branch.Branch("y:z"));
            Assert.Throws<InvalidEntryPayloadKey>(() => branch.Branch(":"));

            EntryPayloadKey k2 = new EntryPayloadKey("Hello:World");
            ClassicAssert.AreEqual("Hello:World", k2.ToString());

            EntryPayloadKey k3 = k2.Branch("Yo");
            ClassicAssert.AreEqual("Hello:World:Yo", k3.ToString());
        }
    }
}
