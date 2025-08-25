using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.GUI.Images;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace MKW.GUI
{
    public class LoginUser
    {
        public required bool IsAdmin { get; init; }
        public required UserId Id { get; init; }
        public required string Name { get; init; }

        public object Icon => IsAdmin ? new Admin() : new User();
    }

    public class LoginWindowViewModel : INotifyPropertyChanged
    {
        private readonly Window window;
        private readonly DatabaseModel database;

        public LoginWindowViewModel(Window window, DatabaseModel database)
        {
            this.window = window;
            this.database = database;

            Users = [];

            LoadUsers();
        }

        private void LoadUsers()
        {
            Users.Clear();

            foreach (LoginUser user in database.EnumerateUsers())
            {
                Users.Add(user);
            }

            SelectedUser = Users[0];
        }

        public ObservableCollection<LoginUser> Users { get; }
        public LoginUser? SelectedUser { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Password { get; set; } = "";

        public string DatabasePath => database.Path;

        public void DoLogin()
        {
            try
            {
                if (SelectedUser == null)
                {
                    throw new Exception("Please select user.");
                }

                database.Authenticate(SelectedUser.Id, Password);
                window.Close();
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        public void ReportError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
