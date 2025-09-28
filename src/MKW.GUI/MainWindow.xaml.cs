using Microsoft.Win32;
using MKW.GUI.CreateDatabaseWizard;
using MKW.GUI.SingleInstance;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace MKW.GUI
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;
        private readonly SingleInstanceApplicationManager manager;

        public MainWindow(MainWindowViewModel viewModel, SingleInstanceApplicationManager manager)
        {
            this.viewModel = viewModel;
            this.manager = manager;

            DataContext = viewModel;

            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            IntPtr windowHandle = new WindowInteropHelper(this).Handle;
            HwndSource source = HwndSource.FromHwnd(windowHandle);

            manager.AddMessageSource(source);
        }

        // File

        private void FileNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                CreateDatabaseWizardViewModel createDatabaseViewModel = viewModel.CreateCreateDatabaseViewModel();
                CreateDatabaseWizard.CreateDatabaseWizard createDatabaseWindow = new CreateDatabaseWizard.CreateDatabaseWizard(createDatabaseViewModel, this);

                createDatabaseWindow.ShowDialog();
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
                    viewModel.CloseTab(tab);
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
            AboutDialogViewModel aboutDialogViewModel = new AboutDialogViewModel();
            AboutDialog window = new AboutDialog(aboutDialogViewModel, GetWindow(this));
            window.ShowDialog();
        }

        private void DoOpenDatabase(string fullPath)
        {
            viewModel.OpenDatabase(fullPath);
        }
    }
}
