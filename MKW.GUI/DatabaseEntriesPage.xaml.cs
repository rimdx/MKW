using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI
{
    public partial class DatabaseEntriesPage : UserControl
    {
        private readonly DatabaseViewModel model;

        public DatabaseEntriesPage(DatabaseViewModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void EntryListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            model.OnEditEntry();
        }

        private void AddEntry_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            model.OnAddEntry();
        }

        private void EditEntry_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            model.OnEditEntry();
        }
    }
}
