using MKW.GUI.Wizard;

namespace MKW.GUI.RequestAccessWizard
{
    public partial class RequestAccessWizardResultsPage : WizardPage
    {
        private readonly RequestAccessWizardViewModel viewModel;

        public RequestAccessWizardResultsPage(RequestAccessWizardViewModel viewModel)
            : base("Database Access Request is Ready!")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void CopyToClipboard_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SaveToFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
