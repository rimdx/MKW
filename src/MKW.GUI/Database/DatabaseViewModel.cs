using MKW.GUI.Model;
using MKW.GUI.RequestAccessWizard;
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
            if (e.MatchProperty(nameof(Database.UnlockedDatabase)))
            {
                UpdateContentView();
            }
        }

        private ViewModelBase UpdateContentView()
        {
            if (Database.UnlockedDatabase != null && contentView is not DatabaseUnlockedViewModel)
            {
                ContentView = new DatabaseUnlockedViewModel(Database.UnlockedDatabase);
            }
            else if (Database.UnlockedDatabase == null && contentView is not DatabaseLockedViewModel)
            {
                ContentView = new DatabaseLockedViewModel(this);
            }

            return contentView;
        }

        public void Dispose()
        {
            Database.Dispose();
        }

        private ViewModelBase contentView;
        public ViewModelBase ContentView
        {
            get => contentView;
            set => SetProperty(ref contentView, value);
        }

        public void LockDatabase()
        {
            Database.Lock();
        }

        public RequestAccessWizardViewModel CreateRequestAccessViewModel()
        {
            return new RequestAccessWizardViewModel(Database);
        }

        public void ReloadDatabaseFile()
        {
            Database.ReloadDatabaseFile();
        }
    }
}
