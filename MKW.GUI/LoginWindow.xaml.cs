using MKW.Core.Client;
using MKW.Core.Storage;
using System.Windows;
using System.Windows.Input;

namespace MKW.GUI
{
    public partial class LoginWindow : Window
    {
        private readonly LoginWindowModel model;

        public ClientSession Client => model.Client;
        public UserSession? User => model.User;

        public LoginWindow(IDatabase database, string databasePath)
        {
            model = new LoginWindowModel(this, database, databasePath);
            DataContext = model;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            model.DoLogin();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(PasswordInput);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            model.Dispose();
        }

        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            model.Password = PasswordInput.Password;
        }
    }
}
