using MKW.Core.Client;
using System.Windows;

namespace MKW.GUI
{
    public partial class NewEntryWindow : Window
    {
        private readonly DatabaseModel database;

        public string Payload { get; set; } = "";

        public NewEntryWindow(DatabaseModel database)
        {
            this.database = database;
            DataContext = this;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            database.CreateEntry(Payload);
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
