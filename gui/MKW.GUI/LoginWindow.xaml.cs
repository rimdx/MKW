using MKW.GUI.RequestAccessWizard;
using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI
{
    public partial class LoginWindow : DialogWindow
    {
        private readonly LoginWindowViewModel model;

        public LoginWindow(LoginWindowViewModel model, Window owner) : base(owner)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (model.DoLogin())
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(this, ex);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RequestAccess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RequestAccessWizardViewModel dialogModel = model.CreateRequestAccessViewModel();
                WizardWindow dialog = new WizardWindow(dialogModel, this);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(this, ex);
            }
        }
    }
}
