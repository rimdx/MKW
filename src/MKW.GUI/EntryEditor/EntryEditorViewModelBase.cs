using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI
{
    public abstract class EntryEditorViewModelBase : ViewModelBase
    {
        public EntryEditorModel Payload { get; }

        public EntryEditorViewModelBase(EntryEditorModel payload)
        {
            Payload = payload;
        }

        public void OnOK()
        {
            SaveEntry(Payload.GetPayload());
        }

        protected abstract void SaveEntry(EntryPayload payload);
    }
}
