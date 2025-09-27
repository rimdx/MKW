using Microsoft.Win32;
using MKW.GUI.CreateDatabaseWizard;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;

            InitializeComponent();
        }

        // File

        private void FileNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                CreateDatabaseWizardViewModel createDatabaseViewModel = viewModel.CreateCreateDatabaseViewModel();
                CreateDatabaseWizard.CreateDatabaseWizard createDatabaseWindow = new CreateDatabaseWizard.CreateDatabaseWizard(createDatabaseViewModel, this);

                createDatabaseWindow.ShowDialog();

                viewModel.OpenDatabase(createDatabaseViewModel);
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(GetWindow(this), ex);
            }
        }

        private void FileOpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                FileDialog dialog = FileDialogUtils.CreateOpenDatabaseDialog();

                if (dialog.ShowDialog() == true)
                {
                    DoOpenDatabase(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(GetWindow(this), ex);
            }
        }

        private void FileOpenRecentCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                string fullPath = (string)e.Parameter;

                DoOpenDatabase(fullPath);
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(GetWindow(this), ex);
            }
        }

        private void RecentFileItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            RecentFileItemViewModel file = (RecentFileItemViewModel)menuItem.DataContext;

            try
            {
                LoginWindowViewModel loginWindowViewModel =
                    viewModel.CreateLoginViewModel(file.FullPath);
                LoginWindow window = new LoginWindow(loginWindowViewModel, GetWindow(this));

                window.ShowDialog();

                viewModel.OpenDatabase(loginWindowViewModel);
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(GetWindow(this), ex);
            }
        }

        private void FileClose_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool canExecute;

            if (e.Parameter != null)
            {
                canExecute = true;
            }
            else if (viewModel.SelectedTab != null)
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
                    tab = viewModel.SelectedTab;
                }

                if (tab != null)
                {
                    viewModel.OnCloseTab(tab);
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(GetWindow(this), ex);
            }
        }

        private void FileExit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        // Help

        private void HelpAbout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AboutDialog window = new AboutDialog(viewModel, GetWindow(this));
            window.ShowDialog();
        }

        private void DoOpenDatabase(string fullPath)
        {
            LoginWindowViewModel loginWindowViewModel = viewModel.CreateLoginViewModel(fullPath);
            LoginWindow window = new LoginWindow(loginWindowViewModel, GetWindow(this));

            window.ShowDialog();

            viewModel.OpenDatabase(loginWindowViewModel);
        }
    }
}
