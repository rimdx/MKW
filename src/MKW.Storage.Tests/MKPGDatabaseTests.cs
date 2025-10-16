using MKW.Core;
using MKW.Storage.MKPG;
using NUnit.Framework.Legacy;

namespace MKW.Storage.Tests
{
    public class MKPGDatabaseTests
    {
        [Test]
        public void SimpleEntryTest()
        {
            Random random = new Random(42);

            using MKPGDatabase database = new MKPGDatabase();

            byte[] data = new byte[128];
            random.NextBytes(data);

            DatabaseEntry dbEntry = new DatabaseEntry
            {
                Id = EntryId.FromGuid(new Guid("{00000000-0000-0000-A2E4-51CAEA3D7ABD}")),
                Data = data,
                Salt = null,
                Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>(),
            };

            database.CreateEntry(dbEntry.Id, dbEntry);

            DatabaseEntry read = database.OpenEntry(dbEntry.Id);

            ClassicAssert.AreEqual(dbEntry.Id, read.Id);
            CollectionAssert.AreEqual(dbEntry.Data.ToArray(), read.Data.ToArray());
            CollectionAssert.AreEqual(dbEntry.Keys, read.Keys);
        }
    }
}
