// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class RfcUserIdTests
    {
        [Test]
        public void SimpleTest()
        {
            RfcUserId obj = new RfcUserId
            {
                Name = "uwu-test-id",
                Comment = "use for mkw testing",
                Email = "test@uwu-mail.com",
            };

            string encoded = RfcUserIdSerializer.Serialize(obj);

            ClassicAssert.AreEqual("uwu-test-id (use for mkw testing) <test@uwu-mail.com>",
                                   encoded);

            RfcUserId decoded = RfcUserIdSerializer.Deserialize(encoded);

            ClassicAssert.AreEqual(obj, decoded);
        }

        [Test]
        public void SimpleNameComment()
        {
            RfcUserId obj = new RfcUserId
            {
                Name = "uwu-test-id",
                Comment = "use for mkw testing",
            };

            string encoded = RfcUserIdSerializer.Serialize(obj);

            ClassicAssert.AreEqual("uwu-test-id (use for mkw testing)",
                                   encoded);

            RfcUserId decoded = RfcUserIdSerializer.Deserialize(encoded);

            ClassicAssert.AreEqual(obj, decoded);
        }

        [Test]
        public void SimpleNameEmail()
        {
            RfcUserId obj = new RfcUserId
            {
                Name = "uwu-test-id",
                Email = "test@uwu-mail.com",
            };

            string encoded = RfcUserIdSerializer.Serialize(obj);

            ClassicAssert.AreEqual("uwu-test-id <test@uwu-mail.com>",
                                   encoded);

            RfcUserId decoded = RfcUserIdSerializer.Deserialize(encoded);

            ClassicAssert.AreEqual(obj, decoded);
        }

        [Test]
        public void SimpleNameOnly()
        {
            RfcUserId obj = new RfcUserId
            {
                Name = "uwu-test-id",
            };

            string encoded = RfcUserIdSerializer.Serialize(obj);

            ClassicAssert.AreEqual("uwu-test-id",
                                   encoded);

            RfcUserId decoded = RfcUserIdSerializer.Deserialize(encoded);

            ClassicAssert.AreEqual(obj, decoded);
        }

        [Test]
        public void MillionSpacesTest()
        {
            RfcUserId obj = new RfcUserId
            {
                Name = "uwu-test-id",
                Comment = "use for mkw testing",
                Email = "test@uwu-mail.com",
            };

            RfcUserId decoded = RfcUserIdSerializer.Deserialize("   uwu-test-id        \n (   use for mkw testing   ) <  test@uwu-mail.com  >    ");

            ClassicAssert.AreEqual(obj, decoded);
        }

        [Test]
        public void InvalidFormatDeserializeTests()
        {
            Assert.Throws<RfcUserIdMalformedException>(
                () => RfcUserIdSerializer.Deserialize("uwu-test-id (use for mkw testing"));

            Assert.Throws<RfcUserIdMalformedException>(
                () => RfcUserIdSerializer.Deserialize("uwu-test-id <test@uwu-mail.com"));

            Assert.Throws<RfcUserIdMalformedException>(
                () => RfcUserIdSerializer.Deserialize("uwu-test-id <test@uwu-mail.com> (use for mkw testing"));

            Assert.Throws<RfcUserIdMalformedException>(
                () => RfcUserIdSerializer.Deserialize("uwu-test-id (use for mkw testing) <test@uwu-mail.com"));

            Assert.Throws<RfcUserIdMalformedException>(
                () => RfcUserIdSerializer.Deserialize("uwu-test-id <test@uwu-mail.com> (use for mkw testing)"));

            Assert.Throws<RfcUserIdMalformedException>(
                () => RfcUserIdSerializer.Deserialize("uwu-test-id (use for mkw testing) <test@uwu-mail.com> abc"));
        }
    }
}
