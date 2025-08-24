using MKW.Core.Client;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI
{
    public partial class DatabasePage : UserControl
    {
        private readonly DatabaseViewModel model;

        public DatabasePage(DatabaseViewModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        public void OnEditEntry()
        {
            if (model.SelectedEntry == null)
            {
                throw new Exception("No entry was selected.");
            }

            using UserEntry entry = model.Database.User!.OpenEntry(model.SelectedEntry.Id);

            EditEntryWindow window = new EditEntryWindow(model.Database,
                                                         entry /* reference */);

            window.ShowDialog();
        }

        private void EntryListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OnEditEntry();
        }
    }
}
