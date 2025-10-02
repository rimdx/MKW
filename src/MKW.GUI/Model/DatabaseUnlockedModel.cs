using MKW.Core;
using System.ComponentModel;

namespace MKW.GUI.Model
{
    public class DatabaseUnlockedModel : ViewModelBase, IDisposable
    {
        public DatabaseModel Database { get; }

        private readonly IUserSession user;
        private readonly IAdminSession? admin;

        public string Path => Database.Path;

        private IReadOnlyCollection<DatabaseEntryModel> entries;
        public IReadOnlyCollection<DatabaseEntryModel> Entries
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

            database.PropertyChanged += Database_PropertyChanged;
        }

        private void Database_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.MatchProperty(nameof(Database.Users)))
            {
                Users = Database.Users;
            }
        }

        private IEnumerable<DatabaseEntryModel> EnumerateEntries()
        {
            if (user != null)
            {
                foreach (IEntrySession entry in user.EnumerateEntries())
                {
                    EntryPayload? payload = entry.OpenPayload();

                    yield return new DatabaseEntryModel
                    {
                        Id = entry.Id,
                        Payload = payload,
                    };
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

        public void CreateEntry(EntryPayload payload)
        {
            using IEntrySession entry = user.CreateEntry();

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

        public void Dispose()
        {
            user?.Dispose();
        }
    }
}
