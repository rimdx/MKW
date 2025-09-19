using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI
{
    public partial class CreateDatabaseMasterPassword : WizardPage
    {
        private readonly CreateDatabaseWizardViewModel viewModel;

        public CreateDatabaseMasterPassword(CreateDatabaseWizardViewModel viewModel)
            : base("Create Master Password")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public override bool Next()
        {
            try
            {
                viewModel.EnsurePassword();
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
