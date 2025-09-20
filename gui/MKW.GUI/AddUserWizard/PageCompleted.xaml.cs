using MKW.GUI.Wizard;
using System.Windows.Controls;

namespace MKW.GUI.AddUserWizard
{
    public partial class PageCompleted : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public PageCompleted(AddUserWizardViewModel viewModel)
            : base("Completed")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
