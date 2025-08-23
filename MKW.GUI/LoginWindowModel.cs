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
        private readonly IDatabase database;
        private readonly ClientSession client;
        private UserSession? user;

        public LoginWindowModel(IDatabase database, string databasePath)
        {
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
                user = client.OpenUser(Password);
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
            database.Dispose();
            client.Dispose();
        }
    }
}
