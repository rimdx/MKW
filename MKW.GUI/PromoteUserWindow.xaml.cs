using System.Windows;

namespace MKW.GUI
{
    public partial class PromoteUserWindow : Window
    {
        private readonly PromoteUserWindowViewModel model;

        public PromoteUserWindow(PromoteUserWindowViewModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (model.OnOK())
            {
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
