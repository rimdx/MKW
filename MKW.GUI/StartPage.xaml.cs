using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    public partial class StartPage : UserControl
    {
        private readonly MainWindowViewModel model;

        public event EventHandler? OpenDatabaseClicked;
        public event EventHandler? NewDatabaseClicked;

        public StartPage(MainWindowViewModel model)
        {
            this.model = model;
            DataContext = model;
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
