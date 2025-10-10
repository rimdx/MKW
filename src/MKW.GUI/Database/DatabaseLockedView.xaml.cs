using System.Windows.Controls;

namespace MKW.GUI.Database
{
    public partial class DatabaseLockedView : UserControl
    {
        private readonly DatabaseLockedViewModel model;

        public DatabaseLockedView(DatabaseLockedViewModel model)
        {
            this.model = model;
            DataContext = model;

            InitializeComponent();
        }
    }
}
