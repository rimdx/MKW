using Microsoft.Win32;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
using System.ComponentModel;

namespace MKW.GUI
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title => "Multi-Key Wallet";

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void NewDatabase()
        {
            FileDialog dialog = new SaveFileDialog
            {
                FileName = "New Database",
                DefaultExt = ".mkw",
                Filter = "Multi-Key Wallet Database File|*.mkw"
            };

            if (dialog.ShowDialog() == true)
            {
                JSONDatabaseSession database = JSONDatabaseSession.Open(
                    dialog.FileName, DatabaseOpenMode.OpenOrCreate);

                LoginWindow window = new LoginWindow(database, dialog.FileName);
                window.Show();
            }
        }

        public void OpenDatabase()
        {
            FileDialog dialog = new OpenFileDialog
            {
                FileName = "New Database",
                DefaultExt = ".mkw",
                Filter = "*.mkw"
            };

            if (dialog.ShowDialog() == true)
            {
                JSONDatabaseSession database = JSONDatabaseSession.Open(
                    dialog.FileName, DatabaseOpenMode.Open);
            }
        }
    }
}
