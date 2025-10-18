// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization;
using MKW.Core.Serialization.KeePassXmlV1;
using MKW.Core.Serialization.KeePassXmlV2;
using NUnit.Framework.Legacy;
using System.Text;

namespace MKW.Tests
{
    public class KeePassXmlSerializerTests
    {
        [Test]
        public void SimpleTest()
        {
            FileStream stream = new FileStream("key_pass_export_v2.xml", FileMode.Open, FileAccess.Read);
            KeePassXmlV2Reader reader = new KeePassXmlV2Reader(stream);

            BackupEntry[] entries = reader.EnumerateEntries().ToArray();

            ClassicAssert.AreEqual(3, entries.Length);
        }

        [Test]
        public void ReadV1()
        {
            FileStream stream = File.OpenRead("key_pass_export_v1.xml");
            KeePassXmlV1Reader reader = new KeePassXmlV1Reader(stream);

            BackupEntry[] entries = reader.GetEntriesEnumerator().ToArray();

            ClassicAssert.AreEqual(5, entries.Length);
        }

        [Test]
        public void ReadWriteV1()
        {
            BackupEntry entry = new BackupEntry(new Dictionary<string, string>
            {
                { "title", "Title 1" },
                { "username", "User 1" },
                { "123%^&<><> \"uihiu niuj (_", "3&*@#!^DNHASJD*782791" },
            });

            byte[] bytes;

            {
                using MemoryStream stream = new MemoryStream();

                using (KeePassXmlV1Writer writer = new KeePassXmlV1Writer(stream))
                {
                    writer.WriteEntry(entry);
                    writer.WriteEntry(entry);
                }

                bytes = stream.ToArray();
                Console.WriteLine(Encoding.UTF8.GetString(bytes));
            }

            {
                using MemoryStream stream = new MemoryStream(bytes);
                using KeePassXmlV1Reader reader = new KeePassXmlV1Reader(stream);

                BackupEntry[] entries = reader.GetEntriesEnumerator().ToArray();

                ClassicAssert.AreEqual(2, entries.Length);

                CollectionAssert.AreEqual(entry.Fields, entries[0].Fields);
            }
        }
    }
}
