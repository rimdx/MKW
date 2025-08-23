using System.Windows.Controls;

namespace MKW.GUI
{
    public partial class DatabasePage : UserControl
    {
        private readonly DatabaseModel model;

        public DatabasePage(DatabaseModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }
    }
}
