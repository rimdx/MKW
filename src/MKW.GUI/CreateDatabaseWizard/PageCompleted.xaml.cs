using MKW.GUI.Wizard;
using System.Diagnostics;
using System.IO;

namespace MKW.GUI.CreateDatabaseWizard
{
    public partial class PageCompleted : WizardPage
    {
        private readonly CreateDatabaseWizardViewModel viewModel;

        public PageCompleted(CreateDatabaseWizardViewModel viewModel)
            : base("Completed")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void RevealDatabaseInExplorer(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = Path.GetDirectoryName(viewModel.DatabasePath),
                UseShellExecute = false,
            });
        }
    }
}
