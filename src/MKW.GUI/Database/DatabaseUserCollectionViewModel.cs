using MKW.GUI.Model;
using System.Collections.ObjectModel;

namespace MKW.GUI.Database
{
    public class DatabaseUserCollectionViewModel : ObservableCollection<DatabaseUserModel>, IDisposable
    {
        private readonly DatabaseModel database;

        public DatabaseUserCollectionViewModel(DatabaseModel database)
        {
            this.database = database;
            database.OnUsersChanged += Database_OnUsersChanged;
            RefreshUsers();
        }

        private void Database_OnUsersChanged(object? sender, EventArgs e)
        {
            RefreshUsers();
        }

        private void RefreshUsers()
        {
            Clear();

            foreach (DatabaseUserModel user in database.Users)
            {
                Add(user);
            }
        }

        public void Dispose()
        {
            database.OnUsersChanged -= Database_OnUsersChanged;
        }
    }
}
