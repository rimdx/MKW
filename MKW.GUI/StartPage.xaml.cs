using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    public partial class StartPage : UserControl
    {
        private readonly MainWindowViewModel model;

        public StartPage(MainWindowViewModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void OpenDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog dialog = new OpenFileDialog
                {
                    DefaultExt = ".mkw",
                    Filter = "Multi-Key Wallet Database File|*.mkw"
                };

                if (dialog.ShowDialog() == true)
                {
                    LoginWindowViewModel loginWindowViewModel =
                        model.CreateLoginViewModel(dialog.FileName);
                    LoginWindow window = new LoginWindow(loginWindowViewModel);

                    window.ShowDialog();

                    model.OpenDatabase(loginWindowViewModel);
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(ex);
            }
        }

        private void NewDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog dialog = new SaveFileDialog
                {
                    FileName = "New Database",
                    DefaultExt = ".mkw",
                    Filter = "Multi-Key Wallet Database File|*.mkw"
                };

                if (dialog.ShowDialog() == true)
                {
                    CreateDatabaseWindowViewModel createDatabaseViewModel =
                        model.CreateCreateDatabaseViewModel(dialog.FileName);

                    CreateDatabaseWindow createDatabaseWindow =
                        new CreateDatabaseWindow(createDatabaseViewModel);

                    createDatabaseWindow.ShowDialog();

                    model.OpenDatabase(createDatabaseViewModel);
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(ex);
            }
        }
    }
}
