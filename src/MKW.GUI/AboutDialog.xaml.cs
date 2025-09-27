using System.Diagnostics;
using System.Windows;

namespace MKW.GUI
{
    public partial class AboutDialog : DialogWindow
    {
        private readonly AboutDialogViewModel model;

        public AboutDialog(AboutDialogViewModel model, Window owner) : base(owner)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GitHubLinkClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/rimdx/MKW",
                UseShellExecute = true
            });
        }
    }
}
