using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.CreateDatabaseWizard
{
    public partial class PageLocation : WizardPage
    {
        private readonly CreateDatabaseWizardViewModel viewModel;

        public PageLocation(CreateDatabaseWizardViewModel viewModel)
            : base("Choose Location")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public override bool Next()
        {
            if (viewModel.Exists())
            {
                MessageBox.Show(Window.GetWindow(this),
                                $"File '{viewModel.DatabasePath}' already exists. Please choose another location.",
                                "File already exists.",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                return false;
            }
            else
            {
                return true;
            }
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = FileDialogUtils.CreateSelectFolderDialog(viewModel.DatabaseDirectory);

            if (dialog.ShowDialog(Window.GetWindow(this)) == true)
            {
                viewModel.DatabaseDirectory = dialog.ResultPath;
            }
        }
    }
}
