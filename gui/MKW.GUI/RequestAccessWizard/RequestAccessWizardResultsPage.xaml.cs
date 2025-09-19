using MKW.GUI.Wizard;
using System.Windows;

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

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(viewModel.RequestString);
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
