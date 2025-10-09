using MKW.GUI.EntryEditor;
using System.Windows;

namespace MKW.GUI.EntryEditor
{
    public partial class NewEntryWindow : DialogWindow
    {
        private readonly NewEntryWindowViewModel model;

        public NewEntryWindow(NewEntryWindowViewModel model, Window owner) : base(owner)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                model.OnOK();
                Close();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
