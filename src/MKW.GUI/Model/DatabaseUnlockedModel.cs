using MKW.Core;
using MKW.GUI.EntryEditor;
using System.ComponentModel;

namespace MKW.GUI.Model
{
    public class DatabaseUnlockedModel : ViewModelBase, IDisposable
    {
        public DatabaseModel Database { get; }
        public CommonEntryPropertiesModel CommonPropertiesModel { get; } 

        private readonly IUserSession user;
        private readonly IAdminSession? admin;

        public string Path => Database.Path;

        private IReadOnlyCollection<EntryPayloadEditorViewModel> entries;
        public IReadOnlyCollection<EntryPayloadEditorViewModel> Entries
        {
            get => entries;
            private set => SetProperty(ref entries, value);
        }

        private IReadOnlyCollection<DatabaseUserModel> users;
        public IReadOnlyCollection<DatabaseUserModel> Users
        {
            get => users;
            private set => SetProperty(ref users, value);
        }

        public DatabaseUnlockedModel(DatabaseModel database, IUserSession user)
        {
            Database = database;

            this.user = user;
            if (user is IAdminSession admin)
            {
                this.admin = admin;
            }

            entries = [.. EnumerateEntries()];
            users = database.Users;

            CommonPropertiesModel = new CommonEntryPropertiesModel(entries);

            database.PropertyChanged += Database_PropertyChanged;
        }

        private void Database_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.MatchProperty(nameof(Database.Users)))
            {
                Users = Database.Users;
            }
        }

        private IEnumerable<EntryPayloadEditorViewModel> EnumerateEntries()
        {
            if (user != null)
            {
                foreach (IEntrySession entry in user.EnumerateEntries())
                {
                    EntryPayload? payload = entry.OpenPayload();

                    yield return new EntryPayloadEditorViewModel(entry.Id, payload,
                                                                 CommonPropertiesModel);
                }
            }
            else
            {
                // empty list
            }
        }

        public void RefreshEntries()
        {
            Entries = [.. EnumerateEntries()];
        }

        public void CreateEntry(EntryId id, EntryPayload payload)
        {
            using IEntrySession entry = user.CreateEntry(id);

            entry.UpdatePayload(payload);

            RefreshEntries();
        }

        public void UpdateEntry(IEntrySession entry, EntryPayload payload)
        {
            entry.UpdatePayload(payload);

            RefreshEntries();
        }

        public void DeleteEntry(EntryId id)
        {
            Database.Database.DeleteEntry(id);
            RefreshEntries();
        }

        public void AddUser(UserAccessRequest request, UserMetadata userMetadata)
        {
            if (admin == null)
            {
                throw new Exception("Not an admin.");
            }

            UserInfo user = admin.CreateUser(request, userMetadata);

            Database.RefreshUsers();
        }

        public UserEditorModel CreateUserEditor(UserId userId)
        {
            UserInfo user = Database.Client.GetUserInfo(userId);
            return new UserEditorModel(this, user);
        }

        public void DeleteUser(UserId id)
        {
            Database.Database.DeleteUser(id);
            Database.RefreshUsers();
        }

        public IEntrySession OpenEntry(EntryId entryId)
        {
            return user.OpenEntry(entryId);
        }

        public KeePassXmlImporter CreateImporter()
        {
            return new KeePassXmlImporter(user);
        }

        public void Dispose()
        {
            user?.Dispose();
        }
    }
}
