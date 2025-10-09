using MKW.GUI.Images;
using MKW.GUI.Model;
using MKW.GUI.Wizard;

namespace MKW.GUI.ImportWizard
{
    public class ImportWizardViewModel : WizardViewModel
    {
        private readonly DatabaseUnlockedModel database;

        private string path;

        public ImportWizardViewModel(DatabaseUnlockedModel database)
            : base("Import Data", ImageMoniker.None)
        {
            this.database = database;

            path = "";

            AddPage(new PageWelcome(this));
            AddPage(new PageFile(this));
            AddPage(new PageConfirmation(this));
            AddPage(new PageCompleted(this));
        }

        public string Path
        {
            get => path;
            set => SetProperty(ref path, value);
        }
    }
}
