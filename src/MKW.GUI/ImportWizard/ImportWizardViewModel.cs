using MKW.GUI.Backup;
using MKW.GUI.Images;
using MKW.GUI.Model;
using MKW.GUI.Wizard;
using System.ComponentModel;
using System.IO;

namespace MKW.GUI.ImportWizard
{
    public class ImportWizardViewModel : WizardViewModel, IDisposable
    {
        private readonly DatabaseUnlockedModel database;

        private string path;
        private bool? isAllSelected;
        private IBackupFormat backupFormat;
        private BackupModel? backupModel;

        public ImportWizardViewModel(DatabaseUnlockedModel database)
            : base("Import Data", ImageMoniker.None)
        {
            this.database = database;

            path = "";
            backupFormat = CommonBackupFormats.KeePassXmlV1;

            AddPage(new PageWelcome(this));
            AddPage(new PageFormat(this));
            AddPage(new PageFile(this));
            AddPage(new PageEntries(this));
            AddPage(new PageConfirmation(this));
            AddPage(new PageCompleted(this));
        }

        public void OpenBackup()
        {
            FileStream stream = File.OpenRead(Path);

            try
            {
                BackupModel = database.OpenBackup(stream, BackupFormat);
            }
            catch (Exception ex)
            {
                throw new Exception($"File cannot be processed. Make sure it is valid and the proper format was chosen.", ex);
            }

            foreach (var entry in BackupModel.Entries)
            {
                entry.PropertyChanged += EntryPropertyChanged;
            }

            IsAllSelected = BackupModel.GetSelectedAll();
        }

        private void EntryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.MatchProperty(nameof(BackupModelEntry.IsSelected)) && BackupModel != null)
            {
                IsAllSelected = BackupModel.GetSelectedAll();
            }
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
            private set
            {
                BackupModel? oldValue = backupModel;

                if (SetProperty(ref backupModel, value))
                {
                    oldValue?.Dispose();
                }
            }
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
