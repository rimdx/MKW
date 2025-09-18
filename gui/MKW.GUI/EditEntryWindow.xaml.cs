using System.Windows;

namespace MKW.GUI
{
    public partial class EditEntryWindow : Window
    {
        private readonly EditEntryWindowViewModel model;

        public EditEntryWindow(EditEntryWindowViewModel model, Window owner)
        {
            this.model = model;
            this.Owner = owner;
            DataContext = model;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (model.OnOK())
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
