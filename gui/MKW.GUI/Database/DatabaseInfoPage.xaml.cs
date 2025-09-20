using System.Windows.Controls;

namespace MKW.GUI.Database
{
    public partial class DatabaseInfoPage : UserControl
    {
        private readonly DatabaseViewModel model;

        public DatabaseInfoPage(DatabaseViewModel model)
        {
            this.model = model;
            DataContext = this;
            InitializeComponent();
        }
    }
}
