using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    public partial class StartPage : UserControl
    {
        public event EventHandler? OpenDatabaseClicked;
        public event EventHandler? NewDatabaseClicked;

        public StartPage()
        {
            InitializeComponent();
        }

        private void OpenDatabase_Click(object sender, RoutedEventArgs e)
        {
            OpenDatabaseClicked?.Invoke(this, EventArgs.Empty);
        }

        private void NewDatabase_Click(object sender, RoutedEventArgs e)
        {
            NewDatabaseClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
