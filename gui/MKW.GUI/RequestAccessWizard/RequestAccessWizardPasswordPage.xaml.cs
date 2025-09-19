using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.RequestAccessWizard
{
    public partial class RequestAccessWizardPasswordPage : WizardPage
    {
        private readonly RequestAccessWizardViewModel viewModel;

        public RequestAccessWizardPasswordPage(RequestAccessWizardViewModel viewModel)
            : base("Create Password")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public override bool Next()
        {
            if (viewModel.Password.Length == 0)
            {
                MessageBox.Show(Window.GetWindow(this), "Password cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!viewModel.IsPasswordMatch)
            {
                MessageBox.Show(Window.GetWindow(this), "Password doesn't match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
