using MKW.Core.Client;
using MKW.Core.Storage;
using System.ComponentModel;
using System.Windows;

namespace MKW.GUI
{
    public enum LoginMode
    {
        UserPassword,
        AdminPassword,
        Anonymous,
    }

    public class LoginWindowModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Window window;
        private readonly MainWindowModel host;
        private readonly IDatabase database;
        private readonly ClientSession client;
        private bool ownsDb = true;

        public LoginWindowModel(Window window, MainWindowModel host, IDatabase database, string databasePath)
        {
            this.window = window;
            this.host = host;
            this.database = database;

            client = ClientSession.Open(database);
            DatabasePath = databasePath;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Password { get; set; } = "";
        public string? ErrorMessage { get; private set; }

        public string DatabasePath { get; }

        public bool IsUserPassword
        {
            get => LoginMode == LoginMode.UserPassword;
            set => LoginMode = LoginMode.UserPassword;
        }

        public bool IsAdminPassword
        {
            get => LoginMode == LoginMode.AdminPassword;
            set => LoginMode = LoginMode.AdminPassword;
        }

        public bool IsAnonymousMode
        {
            get => LoginMode == LoginMode.Anonymous;
            set => LoginMode = LoginMode.Anonymous;
        }

        private LoginMode _loginMode = LoginMode.UserPassword;
        public LoginMode LoginMode
        {
            get => _loginMode;
            set
            {
                _loginMode = value;

                OnPropertyChanged(nameof(IsUserPassword));
                OnPropertyChanged(nameof(IsAdminPassword));
                OnPropertyChanged(nameof(IsAnonymousMode));
            }
        }

        public void DoLogin()
        {
            try
            {
                UserSession user = client.OpenUser(Password);

                host.Database = new DatabaseModel(client, user);

                ownsDb = false;
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
                client.Dispose();
            }
        }
    }
}
