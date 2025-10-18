// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using NUnit.Framework.Legacy;

namespace MKW.Common.Tests
{
    public class Base16ConvertTests
    {
        [Test]
        public void SimpleTest()
        {
            byte[] buf = [1, 2, 3];
            string str = Base16Convert.GetString(buf);

            Console.WriteLine(str);

            byte[] buf2 = Base16Convert.GetBytes(str);
            CollectionAssert.AreEqual(buf, buf2);
        }

        [Test]
        public void RejectStringWithOddLength()
        {
            Assert.Throws<ArgumentException>(() => Base16Convert.GetBytes("BEFFA"));
        }
    }
}
