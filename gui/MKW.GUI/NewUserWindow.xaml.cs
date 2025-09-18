using System.Windows;

namespace MKW.GUI
{
    public partial class NewUserWindow : DialogWindow
    {
        private readonly NewUserWindowViewModel model;

        public NewUserWindow(NewUserWindowViewModel model, Window owner) : base(owner)
        {
            this.model = model;
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
