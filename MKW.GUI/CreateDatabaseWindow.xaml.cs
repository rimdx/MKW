using System.Windows;
using System.Windows.Input;

namespace MKW.GUI
{
    /// <summary>
    /// Interaction logic for CreateDatabaseWindow.xaml
    /// </summary>
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
            if (model.DoCreateDatabase())
            {
                Close();
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
