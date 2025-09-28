using MKW.GUI.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MKW.GUI.Database
{
    public class DatabaseUserCollectionViewModel : ObservableCollection<DatabaseUserModel>, IDisposable
    {
        private readonly DatabaseModel database;

        public DatabaseUserCollectionViewModel(DatabaseModel database)
        {
            this.database = database;
            database.PropertyChanged += Database_PropertyChanged;
            RefreshUsers();
        }

        private void Database_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.MatchProperty(nameof(database.Users)))
            {
                RefreshUsers();
            }
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
            database.PropertyChanged -= Database_PropertyChanged;
        }
    }
}
