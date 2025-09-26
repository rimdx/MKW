using MKW.Core;

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

        public DatabaseUnlockedModel(DatabaseModel database, IUserSession user)
        {
            Database = database;

            this.user = user;
            if (user is IAdminSession admin)
            {
                this.admin = admin;
            }

            entries = [.. EnumerateEntries()];
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
            using IEntrySession entry = user.CreateEntry();
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
            Database.Database.DeleteEntry(id);
            RefreshEntries();
        }

        internal Trust GetTrust(UserInfo user)
        {
            return this.user.VerifyTrust(user.Id) ? Trust.ExplicitTrust : Trust.None;
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
