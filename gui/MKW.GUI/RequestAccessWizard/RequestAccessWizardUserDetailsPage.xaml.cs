using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.RequestAccessWizard
{
    public partial class RequestAccessWizardUserDetailsPage : WizardPage
    {
        private readonly RequestAccessWizardViewModel viewModel;

        public RequestAccessWizardUserDetailsPage(RequestAccessWizardViewModel viewModel) : base("User details")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public override bool Next()
        {
            if (viewModel.UserName.Length == 0)
            {
                MessageBox.Show(Window.GetWindow(this), "User ID cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return base.Next();
        }
    }
}
