using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.CreateDatabaseWizard
{
    public partial class PageConfirmation : WizardPage
    {
        private CreateDatabaseWizardViewModel viewModel;

        public PageConfirmation(CreateDatabaseWizardViewModel viewModel)
            : base("Confirm Database Creation")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public override bool Next()
        {
            try
            {
                viewModel.DoCreate();
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
