using System.Windows.Controls;

namespace MKW.GUI
{
    public partial class StartPage : UserControl
    {
        private readonly MainWindowViewModel model;

        public StartPage(MainWindowViewModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }
    }
}
