using MKW.GUI.Model;
using System.Collections.ObjectModel;

namespace MKW.GUI
{
    public class DatabaseUserCollectionViewModel : ObservableCollection<DatabaseUserModel>, IDisposable
    {
        private readonly DatabaseModel database;

        public DatabaseUserCollectionViewModel(DatabaseModel database)
        {
            database.OnUsersChanged += Database_OnUsersChanged;
            RefreshUsers();
            this.database = database;
        }

        private void Database_OnUsersChanged(object? sender, EventArgs e)
        {
            RefreshUsers();
        }

        private void RefreshUsers()
        {
            Clear();

            foreach (DatabaseUserModel user in database.EnumerateUsers())
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
