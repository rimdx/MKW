using MKW.GUI.Model;
using System.Windows.Controls;
using System.Windows.Input;

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
