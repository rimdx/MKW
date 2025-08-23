using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.GUI.Images;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    public enum LoginMode
    {
        UserPassword,
        AdminPassword,
        Anonymous,
    }

    public class LoginUser
    {
        public required bool IsAdmin { get; init; }
        public required Guid Id { get; init; }
        public required string Name { get; init; }

        public object Icon => IsAdmin ? new Admin() : new User();
    }

    public class LoginWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Window window;
        private readonly IDatabase database;

        public ClientSession Client { get; }
        public UserSession? User { get; private set; }

        private bool ownsDb = true;

        public LoginWindowViewModel(Window window, IDatabase database, string databasePath)
        {
            this.window = window;
            this.database = database;

            Users = [];

            Client = ClientSession.Open(database /* move */, true);
            DatabasePath = databasePath;

            LoadUsers();
        }

        private void LoadUsers()
        {
            Users.Clear();

            Users.Add(new LoginUser
            {
                Id = Guid.Empty,
                IsAdmin = true,
                Name = "Admin"
            });

            foreach (UserInfo user in Client.EnumerateUsersTrust())
            {
                Users.Add(new LoginUser
                {
                    Id = user.Id,
                    IsAdmin = false,
                    Name = "User"
                });
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

        public string DatabasePath { get; }

        public void DoLogin()
        {
            try
            {
                if (SelectedUser == null)
                {
                    throw new Exception("Please select user.");
                }

                User = Client.OpenUser(SelectedUser.Id, Password);
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

        public void Dispose()
        {
            if (ownsDb)
            {
                database.Dispose();
                Client.Dispose();
            }
        }
    }
}
