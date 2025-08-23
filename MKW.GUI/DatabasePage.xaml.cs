using System.Windows.Controls;

namespace MKW.GUI
{
    public partial class DatabasePage : UserControl
    {
        private readonly DatabaseModel model;

        public DatabasePage(DatabaseModel database)
        {
            model = database;
            DataContext = model;
            InitializeComponent();
        }
    }
}
