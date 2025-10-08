using MKW.GUI.Images;
using MKW.GUI.Wizard;

namespace MKW.GUI.ImportWizard
{
    public class ImportWizardViewModel : WizardViewModel
    {
        public ImportWizardViewModel()
            : base("Import Data", ImageMoniker.None)
        {
            AddPage(new PageWelcome(this));
            AddPage(new PageFile(this));
            AddPage(new PageConfirmation(this));
            AddPage(new PageCompleted(this));
        }

        public void Confirm()
        {
            throw new NotImplementedException();
        }
    }
}
