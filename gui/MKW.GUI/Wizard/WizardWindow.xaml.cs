using System.ComponentModel;
using System.Windows;

namespace MKW.GUI.Wizard
{
    public partial class WizardWindow : DialogWindow
    {
        private readonly WizardViewModel viewModel;

        public WizardWindow(WizardViewModel viewModel, Window owner)
        {
            this.viewModel = viewModel;
            Owner = owner;

            DataContext = viewModel;
            InitializeComponent();

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            UpdatePageContent();
        }

        private void UpdatePageContent()
        {
            PageContent.Content = viewModel.CurrentPage;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.MatchProperty(nameof(viewModel.CurrentPage)))
            {
                UpdatePageContent();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.Back();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.Next();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (viewModel.Finish())
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (viewModel.Cancel())
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
