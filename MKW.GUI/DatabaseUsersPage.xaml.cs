using System.Windows.Controls;

namespace MKW.GUI
{
    public partial class DatabaseUsersPage : UserControl
    {
        private readonly DatabaseViewModel model;

        public DatabaseUsersPage(DatabaseViewModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
