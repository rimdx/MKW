using System.Windows.Controls;

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

            EntriesPage.Content = new DatabaseEntriesPage(model);
            UsersPage.Content = new DatabaseUsersPage(model);
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
