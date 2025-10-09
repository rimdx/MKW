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
        private BackupFormat backupFormat;

        public ImportWizardViewModel(DatabaseUnlockedModel database)
            : base("Import Data", ImageMoniker.None)
        {
            this.database = database;

            path = "";
            backupFormat = BackupFormat.KeePassXmlV2;

            AddPage(new PageWelcome(this));
            AddPage(new PageFormat(this));
            AddPage(new PageFile(this));
            AddPage(new PageEntries(this));
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

            BackupModel backup = database.OpenBackup(stream);

            backup.Import();
        }

        public BackupFormat BackupFormat
        {
            get => backupFormat;
            set => SetProperty(ref backupFormat, value);
        }
    }
}
