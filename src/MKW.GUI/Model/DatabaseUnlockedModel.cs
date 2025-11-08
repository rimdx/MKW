// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.GUI.Backup;
using System.ComponentModel;

namespace MKW.GUI.Model
{
    public class DatabaseUnlockedModel : ViewModelBase, IDisposable
    {
        public DatabaseModel Database { get; }
        public CommonEntryPropertiesModel CommonPropertiesModel { get; }

        private readonly IUserSession user;
        private readonly IAdminSession? admin;

        // For our test-suite only!
        public IUserSession UserUnsafe => user;

        public string Path => Database.Path;

        private IReadOnlyCollection<EntryEditorModel> entries;
        public IReadOnlyCollection<EntryEditorModel> Entries
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

        private IEnumerable<EntryEditorModel> EnumerateEntries()
        {
            if (user != null)
            {
                foreach (KeyValuePair<EntryId, EntryPayload?> entry in user.EnumerateEntries())
                {
                    yield return new EntryEditorModel(entry.Key, entry.Value,
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
            user.CreateEntry(id, payload);
            RefreshEntries();
        }

        public void UpdateEntry(EntryId entryId, EntryPayload payload)
        {
            user.UpdateEntry(entryId, payload);
            RefreshEntries();
        }

        public void DeleteEntry(EntryId id)
        {
            using (Storage.IDatabaseNG.ITransaction transaction = Database.Database.BeginTransaction())
            {
                transaction.DeleteEntry(id);
                transaction.Commit();
            }

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
            using (Storage.IDatabaseNG.ITransaction transaction = Database.Database.BeginTransaction())
            {
                transaction.DeleteUser(id);
                transaction.Commit();
            }

            Database.RefreshUsers();
        }

        public EntryPayload OpenEntry(EntryId entryId)
        {
            return user.OpenEntry(entryId);
        }

        public BackupImportModel CreateImporter(IBackupReader backup)
        {
            return new BackupImportModel(this, user, backup);
        }

        public void Dispose()
        {
            user?.Dispose();
        }
    }
}
