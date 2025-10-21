// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core;
using MKW.GUI.Backup;
using MKW.GUI.Model;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;
using System.Diagnostics;

namespace MKW.GUI.Tests
{
    public class BackupModelTests
    {
        [Test]
        public void SimpleTest()
        {
            KeePassXmlV1BackupFormat format = CommonBackupFormats.KeePassXmlV1;

            EntryPayload entry1 = new EntryPayload();
            EntryId entry1key = EntryId.Create();
            entry1.SetProperty(CommonEntryPropertiesModel.Title.Key, "entry1");

            EntryPayload entry2 = new EntryPayload();
            EntryId entry2key = EntryId.Create();
            entry2.SetProperty(CommonEntryPropertiesModel.Title.Key, "entry2");

            EntryPayload entry3 = new EntryPayload();
            EntryId entry3key = EntryId.Create();
            entry3.SetProperty(CommonEntryPropertiesModel.Title.Key, "entry3");

            using ClientSandBox sbox = new ClientSandBox(false);
            using DatabaseModel database = DatabaseModel.Create(sbox.Crypto, sbox.DatabasePath, sbox.AdminSecret);
            using DatabaseUnlockedModel unlocked = database.Unlock(UserId.Admin(), sbox.AdminSecret);

            unlocked.CreateEntry(entry1key, entry1);
            unlocked.CreateEntry(entry2key, entry2);
            unlocked.CreateEntry(entry3key, entry3);
            ClassicAssert.AreEqual(3, unlocked.Entries.Count);

            using MemoryStream backupfile = new MemoryStream();

            {
                using BackupExportModel exporter = new BackupExportModel(unlocked);
                exporter.Entries[0].IsSelected = false;
                exporter.Entries[1].IsSelected = true;
                exporter.Entries[2].IsSelected = true;
                using IBackupWriter writer = format.OpenWrite(new StreamDisown(backupfile));
                exporter.Export(writer);
            }

            {
                backupfile.Seek(0, SeekOrigin.Begin);
                using IBackupReader reader = format.OpenRead(new StreamDisown(backupfile));
                using BackupImportModel importer = unlocked.CreateImporter(reader);

                ClassicAssert.AreEqual(2, importer.Entries.Count);

                ClassicAssert.IsFalse(importer.Entries[0].IsSelected);
                ClassicAssert.IsFalse(importer.Entries[1].IsSelected);

                importer.Import();
            }

            unlocked.DeleteEntry(entry2key);
            ClassicAssert.AreEqual(2, unlocked.Entries.Count);

            {
                backupfile.Seek(0, SeekOrigin.Begin);
                using IBackupReader reader = format.OpenRead(new StreamDisown(backupfile));
                using BackupImportModel importer = unlocked.CreateImporter(reader);

                ClassicAssert.AreEqual(2, importer.Entries.Count);

                ClassicAssert.IsTrue(importer.Entries[0].IsSelected);
                ClassicAssert.IsFalse(importer.Entries[1].IsSelected);

                importer.Import();
            }

            ClassicAssert.AreEqual(3, unlocked.Entries.Count);
        }

        [Test]
        [TestCase(100)]
        public void PerformanceTest(int count)
        {
            KeePassXmlV1BackupFormat format = CommonBackupFormats.KeePassXmlV1;
            Stopwatch timer = new Stopwatch();

            using ClientSandBox sbox = new ClientSandBox(false);
            using DatabaseModel database = DatabaseModel.Create(sbox.Crypto, sbox.DatabasePath, sbox.AdminSecret);
            using DatabaseUnlockedModel unlocked = database.Unlock(UserId.Admin(), sbox.AdminSecret);

            timer.Restart();

            for (int i = 0; i < count; i++)
            {
                EntryPayload payload = new EntryPayload();
                payload.SetProperty(CommonEntryPropertiesModel.Title.Key, $"entry{i}");

                unlocked.UserUnsafe.CreateEntry(payload);
            }
            unlocked.RefreshEntries();

            timer.Stop();
            Console.WriteLine($"Creating gazillion entries took {timer.ElapsedMilliseconds} ms");

            using MemoryStream backupfile = new MemoryStream();

            {
                timer.Restart();
                using BackupExportModel exporter = new BackupExportModel(unlocked);
                using IBackupWriter writer = format.OpenWrite(new StreamDisown(backupfile));
                exporter.Export(writer);
                timer.Stop();
                Console.WriteLine($"Export took {timer.ElapsedMilliseconds} ms");
            }

            EntryId[] ids = [.. unlocked.Entries.Select(e => e.Id)];
            foreach (EntryId id in ids)
            {
                unlocked.UserUnsafe.DeleteEntry(id);
            }
            unlocked.RefreshEntries();

            {
                backupfile.Seek(0, SeekOrigin.Begin);

                using IBackupReader reader = format.OpenRead(new StreamDisown(backupfile));

                timer.Restart();
                using BackupImportModel importer = unlocked.CreateImporter(reader);
                timer.Stop();
                Console.WriteLine($"Load took {timer.ElapsedMilliseconds} ms");

                ClassicAssert.AreEqual(count, importer.Entries.Count);

                timer.Restart();
                importer.Import();
                timer.Stop();
                Console.WriteLine($"Import took {timer.ElapsedMilliseconds} ms");
            }
        }
    }
}
