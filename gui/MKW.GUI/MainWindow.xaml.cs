using Microsoft.Win32;
using MKW.GUI.Wizard;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel model;

        public MainWindow()
        {
            model = new MainWindowViewModel();
            DataContext = model;

            InitializeComponent();
        }

        // File

        private void NewDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateDatabaseWindowViewModel createDatabaseViewModel = model.CreateCreateDatabaseViewModel();
                WizardWindow createDatabaseWindow = new WizardWindow(createDatabaseViewModel, this);

                createDatabaseWindow.ShowDialog();

                model.OpenDatabase(createDatabaseViewModel);
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
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

        private void RecentFileItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            RecentFileItemViewModel file = (RecentFileItemViewModel)menuItem.DataContext;

            try
            {
                LoginWindowViewModel loginWindowViewModel =
                    model.CreateLoginViewModel(file.FullPath);
                LoginWindow window = new LoginWindow(loginWindowViewModel, Window.GetWindow(this));

                window.ShowDialog();

                model.OpenDatabase(loginWindowViewModel);
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (model.SelectedTab != null)
                {
                    model.OnCloseTab(model.SelectedTab);
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void TabClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FrameworkElement element = (Control)sender;
                DatabaseTabItemViewModel tab = (DatabaseTabItemViewModel)element.DataContext;
                model.OnCloseTab(tab);
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Help

        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog window = new AboutDialog(model);
            window.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            model.Dispose();
        }
    }
}
