using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MKW.GUI
{
    public partial class CreateDatabaseWindow : Window
    {
        private readonly CreateDatabaseWindowViewModel viewModel;

        public CreateDatabaseWindow(CreateDatabaseWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (viewModel.Exists())
                {
                    MessageBoxResult result = MessageBox.Show(this,
                                                              $"{viewModel.DatabaseName} already exists. Do you want to replace it?",
                                                              "Confirm Creation",
                                                              MessageBoxButton.YesNo,
                                                              MessageBoxImage.Warning);

                    if (result != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }

                if (viewModel.DoCreateDatabase())
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(ex);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(PasswordInput);
        }
    }
}
