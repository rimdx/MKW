using MKW.GUI.Model;
using System.ComponentModel;

namespace MKW.GUI.Database
{
    public class DatabaseViewModel : ViewModelBase, IDisposable
    {
        public DatabaseModel Database { get; }

        public DatabaseViewModel(DatabaseModel database)
        {
            Database = database;
            database.PropertyChanged += Database_PropertyChanged;
            contentView = UpdateContentView();
        }

        private void Database_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.MatchProperty(nameof(Database.User)))
            {
                UpdateContentView();
            }
        }

        private object UpdateContentView()
        {
            if (Database.User != null && contentView is not DatabaseUnlockedView)
            {
                ContentView = new DatabaseUnlockedView(new DatabaseUnlockedViewModel(this));
            }
            else if (Database.User == null && contentView is not DatabaseLockedView)
            {
                ContentView = new DatabaseLockedView(new DatabaseLockedViewModel(this));
            }

            return contentView;
        }

        public void Dispose()
        {
            Database.Dispose();
        }

        private object contentView;

        public object ContentView
        { 
            get => contentView;
            set => SetProperty(ref contentView, value);
        }

        public void LockDatabase()
        {
            Database.Lock();
        }

        public LoginWindowViewModel CreateLoginViewModel()
        {
            return new LoginWindowViewModel(Database);
        }
    }
}
