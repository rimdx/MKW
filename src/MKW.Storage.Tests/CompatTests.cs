// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Core.Client;
using MKW.Cryptography.Loader;
using MKW.Storage.JSON;
using NUnit.Framework.Legacy;

namespace MKW.Storage.Tests
{
    public class CompatTests
    {
        [Test]
        [TestCase("test_json_database_v2.mkw")]
        [TestCase("test_json_database_v2_formatted.mkw")]
        public void LoadLockedJsonDatabase(string path)
        {
            using JSONDatabaseSession database = JSONDatabaseSession.Open(path);

            ClassicAssert.AreEqual(2, database.EnumerateEntries().Count());
            ClassicAssert.AreEqual(2, database.EnumerateUsers().Count());

            using ClientSession client = ClientSession.Open(database, BouncyCastleLoader.GetProvider());

            using IAdminSession admin = client.OpenAdmin("123");

            ClassicAssert.NotNull(admin.EnumerateEntries().ToArray()[0].OpenPayload());
        }
    }
}
