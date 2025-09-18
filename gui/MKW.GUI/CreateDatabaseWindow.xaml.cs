using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MKW.GUI
{
    public partial class CreateDatabaseWindow : Window
    {
        private readonly CreateDatabaseWindowViewModel model;

        public CreateDatabaseWindow(CreateDatabaseWindowViewModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(model.DatabasePath))
                {
                    MessageBoxResult result = MessageBox.Show(this,
                                                              $"{model.DatabaseName} already exists. Do you want to replace it?",
                                                              "Confirm Creation",
                                                              MessageBoxButton.YesNo,
                                                              MessageBoxImage.Warning);

                    if (result != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }

                if (model.DoCreateDatabase())
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
