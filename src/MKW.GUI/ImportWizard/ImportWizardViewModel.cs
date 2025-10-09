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
        private BackupModel? backupModel;

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

        public void OpenBackup()
        {
            using FileStream stream = File.OpenRead(Path);
            BackupModel = database.OpenBackup(stream);
        }

        public void Confirm()
        {
            if (BackupModel is null)
            {
                throw new InvalidOperationException("Backup model is not initialized.");
            }

            BackupModel.Import();
        }

        public BackupModel? BackupModel
        {
            get => backupModel;
            private set => SetProperty(ref backupModel, value);
        }

        public string Path
        {
            get => path;
            set => SetProperty(ref path, value);
        }

        public BackupFormat BackupFormat
        {
            get => backupFormat;
            set => SetProperty(ref backupFormat, value);
        }
    }
}
