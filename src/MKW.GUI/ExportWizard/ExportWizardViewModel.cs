using MKW.GUI.Backup;
using MKW.GUI.Images;
using MKW.GUI.Model;
using MKW.GUI.Wizard;
using System.ComponentModel;
using System.IO;

namespace MKW.GUI.ExportWizard
{
    public class ExportWizardViewModel : WizardViewModel, IDisposable
    {
        private readonly DatabaseUnlockedModel database;

        private string path;
        private bool? isAllSelected;
        private IBackupFormat backupFormat;

        public BackupModel BackupModel { get; }

        public ExportWizardViewModel(DatabaseUnlockedModel database)
            : base("Export Data", ImageMoniker.None)
        {
            this.database = database;

            path = "";
            backupFormat = CommonBackupFormats.KeePassXmlV1;
            BackupModel = new BackupModel(database);

            AddPage(new PageWelcome(this));
            AddPage(new PageEntries(this));
            AddPage(new PageFormat(this));
            AddPage(new PageFile(this));
            AddPage(new PageConfirmation(this));
            AddPage(new PageCompleted(this));

            isAllSelected = BackupModel.GetSelectedAll();
        }

        private void EntryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.MatchProperty(nameof(BackupModelEntry.IsSelected)))
            {
                IsAllSelected = BackupModel.GetSelectedAll();
            }
        }

        public void VerifyPath()
        {
            FileInfo file = new FileInfo(Path);
            DirectoryInfo dir = file.Directory;

            if (!dir.Exists)
            {
                throw new Exception($"The system cannot find the path specified.");
            }
        }

        public void Confirm()
        {
            using FileStream file = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write);
            using IBackupWriter writer = BackupFormat.OpenWrite(file);

            BackupModel.Export(writer);
        }

        public string Path
        {
            get => path;
            set => SetProperty(ref path, value);
        }

        public bool? IsAllSelected
        {
            get => isAllSelected;
            set
            {
                if (SetProperty(ref isAllSelected, value) &&
                    BackupModel != null && value != null)
                {
                    BackupModel.SetSelectedAll(value ?? false);
                }
            }
        }

        public IBackupFormat BackupFormat
        {
            get => backupFormat;
            set => SetProperty(ref backupFormat, value);
        }

        public void Dispose()
        {
            BackupModel?.Dispose();
        }
    }
}
