using MKW.GUI.Model;
using System.Windows;

namespace MKW.GUI
{
    public partial class PromoteUserWindow : Window
    {
        private readonly DatabaseModel database;

        public PromoteUserWindow(DatabaseModel database)
        {
            this.database = database;
            DataContext = this;
            InitializeComponent();
        }

        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
        }

        private void PasswordRepeatInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
