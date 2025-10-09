using MKW.Core.Serialization.KeePassXML;
using MKW.GUI.Images;
using MKW.GUI.Model;
using MKW.GUI.Wizard;
using System.IO;

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

        public void Confirm()
        {
            using FileStream stream = File.OpenRead(Path);
            KeePassXmlReader reader = new KeePassXmlReader(stream);

            KeePassXmlImporter importer = database.CreateImporter();

            importer.Import(reader);
        }
    }
}
