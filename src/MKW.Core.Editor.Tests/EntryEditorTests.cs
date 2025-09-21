using MKW.Core.Client;
using MKW.Core.Storage;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;

namespace MKW.Core.Editor.Tests
{
    public class EntryEditorTests
    {
        [Test]
        [Ignore("todo")]
        public void SimpleTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            using IUserSession user = sbox.CreateUser(client, "123", out _);

            EntryId id = EntryId.Create();
            using EntryEditor editor = new EntryEditor(user.CreateEntry(id));

            {
                using IEntrySession entry = user.OpenEntry(id);
                ClassicAssert.AreEqual(null,
                                       entry.OpenPayload());
                ClassicAssert.AreEqual(null,
                                       editor.OpenPayload());
            }

            editor.UpdatePayload(new EntryPayload("data1"));

            {
                using IEntrySession entry = user.OpenEntry(id);
                ClassicAssert.AreEqual(null,
                                       entry.OpenPayload());
                ClassicAssert.AreEqual(new EntryPayload("data1"),
                                       editor.OpenPayload());
            }

            editor.Commit();

            {
                using IEntrySession entry = user.OpenEntry(id);
                ClassicAssert.AreEqual(new EntryPayload("data1"),
                                       entry.OpenPayload());
                ClassicAssert.AreEqual(new EntryPayload("data1"),
                                       editor.OpenPayload());
            }

            editor.Commit();
            editor.UpdatePayload(new EntryPayload("data2"));

            {
                using IEntrySession entry = user.OpenEntry(id);
                ClassicAssert.AreEqual(new EntryPayload("data1"),
                                       entry.OpenPayload());
                ClassicAssert.AreEqual(new EntryPayload("data2"),
                                       editor.OpenPayload());
            }
        }
    }
}
