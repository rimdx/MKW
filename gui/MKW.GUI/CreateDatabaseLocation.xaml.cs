using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI
{
    public partial class CreateDatabaseLocation : WizardPage
    {
        private readonly CreateDatabaseWindowViewModel viewModel;

        public CreateDatabaseLocation(CreateDatabaseWindowViewModel viewModel)
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
                MessageBoxResult result = MessageBox.Show(Window.GetWindow(this),
                                                          $"{viewModel.DatabaseName} already exists. Do you want to replace it?",
                                                          "Confirm Creation",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);

                return result == MessageBoxResult.Yes;
            }
            else
            {
                return true;
            }
        }
    }
}
