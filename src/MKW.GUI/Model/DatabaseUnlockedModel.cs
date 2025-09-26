using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;

namespace MKW.GUI.Model
{
    public class DatabaseUnlockedModel : ViewModelBase, IDisposable
    {
        private IDatabase database;
        private IUserSession? user;
        private IAdminSession? admin;

        public string Path { get; }

        public ClientSession Client { get; }

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

        public IUserSession? User
        {
            get => user;
            private set => SetProperty(ref user, value);
        }

        public IAdminSession? Admin
        {
            get => admin;
            private set => SetProperty(ref admin, value);
        }

        private DatabaseUnlockedModel(IDatabase database, string path, ClientSession client)
        {
            this.database = database;
            Path = path;
            Client = client;

            entries = [.. EnumerateEntries()];
            users = [.. EnumerateUsers()];
        }

        public static DatabaseUnlockedModel Open(string path)
        {
            JSONDatabaseSession db = JSONDatabaseSession.Open(path);
            ClientSession client = ClientSession.Open(db);
            return new DatabaseUnlockedModel(db, path, client);
        }

        public static DatabaseUnlockedModel Create(string path, string adminPassword)
        {
            UserMetadata metadata = new UserMetadata // todo
            {
                DisplayName = "",
                UserId = ""
            };

            JSONDatabaseSession db = JSONDatabaseSession.Create(path);
            ClientSession client = ClientSession.Create(db, adminPassword, metadata);

            DatabaseUnlockedModel model = new DatabaseUnlockedModel(db, path, client);

            model.Admin = model.Client.OpenAdmin(adminPassword);
            model.User = model.Admin;

            return model;
        }

        public void Unlock(UserId id, string password)
        {
            User = Client.OpenUser(id, password);

            if (User is IAdminSession admin)
            {
                Admin = admin;
            }

            RefreshEntries();
        }

        public void Lock()
        {
            User = null;
            Admin = null;
        }

        private IEnumerable<DatabaseEntryModel> EnumerateEntries()
        {
            if (User != null)
            {
                foreach (IEntrySession entry in User.EnumerateEntries())
                {
                    EntryPayload? payload = entry.OpenPayload();

                    yield return new DatabaseEntryModel
                    {
                        Id = entry.Id,
                        Payload = payload?.ToString()
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

        public void CreateEntry(string payload)
        {
            using IEntrySession entry = User!.CreateEntry();
            entry.UpdatePayload(new EntryPayload(payload));
            RefreshEntries();
        }

        public void UpdateEntry(IEntrySession entry, string text)
        {
            entry.UpdatePayload(new EntryPayload(text));
            RefreshEntries();
        }

        public void DeleteEntry(EntryId id)
        {
            database.DeleteEntry(id);
            RefreshEntries();
        }

        private Trust GetTrust(UserInfo user)
        {
            if (User == null)
            {
                return Trust.Unknown;
            }
            else
            {
                // TODO:
                return User.VerifyTrust(user.Id) ? Trust.ExplicitTrust : Trust.None;
            }
        }

        public void AddUser(UserAccessRequest request, UserMetadata userMetadata)
        {
            if (Admin == null)
            {
                throw new Exception("Not an admin.");
            }

            UserInfo user = Admin.CreateUser(request, userMetadata);

            RefreshUsers();
        }

        public UserEditorModel CreateUserEditor(UserId userId)
        {
            UserInfo user = Client.GetUserInfo(userId);
            return new UserEditorModel(this, user);
        }

        public void DeleteUser(UserId id)
        {
            database.DeleteUser(id);
            RefreshUsers();
        }

        private IEnumerable<DatabaseUserModel> EnumerateUsers()
        {
            foreach (UserInfo user in Client.EnumerateUsers())
            {
                yield return new DatabaseUserModel(user, GetTrust(user));
            }
        }

        private void RefreshUsers()
        {
            Users = [.. EnumerateUsers()];
        }

        public void Dispose()
        {
            User?.Dispose();
            Client?.Dispose();
            database?.Dispose();
        }
    }
}
