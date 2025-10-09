using MKW.Core;

namespace MKW.GUI.Model
{
    public sealed class BackupModelEntry : ViewModelBase
    {
        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public EntryPayload Payload { get; }

        public BackupModelEntry(EntryPayload payload)
        {
            isSelected = true;
            Payload = payload;
        }
    }
}
