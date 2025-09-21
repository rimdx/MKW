using System.Windows.Controls;

namespace MKW.GUI.Wizard
{
    public abstract partial class WizardPage : UserControl
    {
        public string Header { get; }

        public WizardPage(string header)
        {
            Header = header;
        }

        public virtual bool Next()
        {
            return true;
        }
    }
}
