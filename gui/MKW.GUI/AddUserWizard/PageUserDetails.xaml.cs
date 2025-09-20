using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.AddUserWizard
{
    public partial class PageUserDetails : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public PageUserDetails(AddUserWizardViewModel viewModel) : base("User details")
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
