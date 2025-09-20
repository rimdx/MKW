using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.AddUserWizard
{
    public partial class PageConfirmation : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public PageConfirmation(AddUserWizardViewModel viewModel)
            : base("Confirmation")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public override bool Next()
        {
            try
            {
                viewModel.DoAddUser();
                return true;
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
                return false;
            }
        }
    }
}
