using MKW.Core.Client;
using MKW.GUI.Model;
using System.Windows;

namespace MKW.GUI
{
    public partial class EditEntryWindow : Window, IDisposable
    {
        private readonly DatabaseModel database;
        private readonly UserEntry entry;

        public EditEntryWindow(DatabaseModel database, UserEntry entry)
        {
            this.database = database;
            this.entry = entry;

            DataContext = this;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            database.UpdateEntry(entry, Payload.Text);
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string? payload = entry.OpenPayload()?.ToString();

            if (payload != null)
            {
                Payload.Text = payload;
            }
        }

        public void Dispose()
        {
            entry.Dispose();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
