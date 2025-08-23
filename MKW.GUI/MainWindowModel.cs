using MKW.Core.Client;
using System.ComponentModel;

namespace MKW.GUI
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title => "Multi-Key Wallet";

        private DatabaseModel? _database;
        public DatabaseModel? Database
        {
            get => _database;
            set
            {
                _database = value;
                OnPropertyChanged(nameof(Database));
            }
        }

        public DatabaseModel OpenDatabase(ClientSession client, UserSession user)
        {
            Database?.Dispose();

            Database = new DatabaseModel(client /* move */,
                                         user /* move */);
            return Database;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
