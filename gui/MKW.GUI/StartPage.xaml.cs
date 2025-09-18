using Microsoft.Win32;
using MKW.GUI.Wizard;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

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
                FileDialog dialog = FileDialogUtils.CreateOpenDatabaseDialog();

                if (dialog.ShowDialog() == true)
                {
                    LoginWindowViewModel loginWindowViewModel =
                        model.CreateLoginViewModel(dialog.FileName);
                    LoginWindow window = new LoginWindow(loginWindowViewModel, Window.GetWindow(this));

                    window.ShowDialog();

                    model.OpenDatabase(loginWindowViewModel);
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void NewDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateDatabaseWindowViewModel createDatabaseViewModel = model.CreateCreateDatabaseViewModel();
                WizardWindow createDatabaseWindow = new WizardWindow(createDatabaseViewModel, Window.GetWindow(this));

                createDatabaseWindow.ShowDialog();

                model.OpenDatabase(createDatabaseViewModel);
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = (Hyperlink)e.Source;
            RecentFileItemViewModel item = (RecentFileItemViewModel)hyperlink.DataContext;

            try
            {
                LoginWindowViewModel loginWindowViewModel = model.CreateLoginViewModel(item.FullPath);
                LoginWindow window = new LoginWindow(loginWindowViewModel, Window.GetWindow(this));

                window.ShowDialog();

                model.OpenDatabase(loginWindowViewModel);
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
