// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using NUnit.Framework.Legacy;
using System.Globalization;

namespace MKW.Common.Tests
{
    [TestFixture]
    [Parallelizable]
    public class PasswordUtilsTests
    {
        [Test]
        [TestCase("", "0.00")]
        [TestCase("1", "3.32")]
        [TestCase("12", "6.64")]
        [TestCase("test", "18.80")]
        [TestCase("TEST", "18.80")]
        [TestCase("Test", "22.80")]
        public void MeasurePasswordEntropySimpleTest(string password, string expected)
        {
            double actual = PasswordUtils.MeasurePasswordEntropy(password);

            ClassicAssert.AreEqual(
                expected.ToString(),
                string.Format(CultureInfo.InvariantCulture, "{0:0.00}", actual));
        }
    }
}
