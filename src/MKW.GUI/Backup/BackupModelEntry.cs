using MKW.Core;

namespace MKW.GUI.Backup
{
    public sealed class BackupModelEntry : ViewModelBase
    {
        private bool isSelected;

        public EntryPayload Payload { get; }

        public BackupModelEntry(EntryPayload payload)
        {
            isSelected = true;
            Payload = payload;
        }

        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public string this[string key]
        {
            get => Payload.GetPropertyOrEmpty(new EntryPayloadKey(key));
        }
    }
}
