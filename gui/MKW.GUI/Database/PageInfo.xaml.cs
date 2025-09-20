using System.Windows.Controls;

namespace MKW.GUI.Database
{
    public partial class PageInfo : UserControl
    {
        private readonly DatabaseViewModel model;

        public PageInfo(DatabaseViewModel model)
        {
            this.model = model;
            DataContext = this;
            InitializeComponent();
        }
    }
}
