using Microsoft.Win32;
using MKW.GUI.Model;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel model;
        private DatabasePage? databasePage;

        public MainWindow()
        {
            model = new MainWindowViewModel();
            model.PropertyChanged += Model_PropertyChanged;
            DataContext = model;

            InitializeComponent();

            UpdateDatabasePage();
        }

        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.Database))
            {
                UpdateDatabasePage();
            }
        }

        private void UpdateDatabasePage()
        {
            if (model.Database == null)
            {
                Database.Content = new StartPage(model);
            }
            else
            {
                databasePage = new DatabasePage(model.Database /* reference */);
                Database.Content = databasePage;
            }
        }

        // File

        private void NewDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog dialog = FileDialogUtils.CreateSaveDatabaseDialog();

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
                ErrorReporter.HandleException(ex);
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
                ErrorReporter.HandleException(ex);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            model.OnCloseDatabase();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Entry

        private void AddEntry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewEntryWindowViewModel viewModel = model.Database!.CreateNewEntryWindowViewModel();
                NewEntryWindow window = new NewEntryWindow(viewModel);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(ex);
            }
        }

        private void EditEntry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using EditEntryWindowViewModel viewModel = model.Database!.CreateEditEntryWindowViewModel();
                EditEntryWindow window = new EditEntryWindow(viewModel);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(ex);
            }
        }

        private void DeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this entry?",
                                                          "Confirm Deletion",
                                                          MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    model.Database!.DeleteEntry();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(ex);
            }

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
