using System.Windows;
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

        private void LoginCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                model.Login();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
