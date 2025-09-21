using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI.Database
{
    public partial class DatabasePage : UserControl
    {
        private readonly DatabaseViewModel model;

        public DatabasePage(DatabaseViewModel model)
        {
            this.model = model;
            DataContext = model;

            InitializeComponent();
        }

        private void LockDatabaseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                model.LockDatabase();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void UnlockDatabaseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                LoginWindowViewModel loginViewModel = model.CreateLoginViewModel();

                LoginWindow loginWindow = new LoginWindow(loginViewModel, Window.GetWindow(this));

                loginWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
