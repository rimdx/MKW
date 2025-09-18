using Microsoft.Win32;
using MKW.GUI.Wizard;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void FileNew_Executed(object sender, ExecutedRoutedEventArgs e)
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

        private void FileOpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                string fullPath;

                if (e.Parameter != null)
                {
                    fullPath = (string)e.Parameter;
                }
                else
                {
                    FileDialog dialog = FileDialogUtils.CreateOpenDatabaseDialog();

                    if (dialog.ShowDialog() != true)
                    {
                        return;
                    }

                    fullPath = dialog.FileName;
                }

                LoginWindowViewModel loginWindowViewModel =
                        model.CreateLoginViewModel(fullPath);
                LoginWindow window = new LoginWindow(loginWindowViewModel, Window.GetWindow(this));

                window.ShowDialog();

                model.OpenDatabase(loginWindowViewModel);
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

        private void FileClose_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool canExecute;

            if (e.Parameter != null)
            {
                canExecute = true;
            }
            else if (model.SelectedTab != null)
            {
                canExecute = true;
            }
            else
            {
                canExecute = false;
            }

            e.CanExecute = canExecute;
        }

        private void FileClose_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                DatabaseTabItemViewModel? tab;
                if (e.Parameter != null)
                {
                    tab = (DatabaseTabItemViewModel)e.Parameter;
                }
                else
                {
                    tab = model.SelectedTab;
                }

                if (tab != null)
                {
                    model.OnCloseTab(tab);
                }
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
