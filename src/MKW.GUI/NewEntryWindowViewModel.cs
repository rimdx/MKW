using MKW.GUI.Model;

namespace MKW.GUI
{
    public class NewEntryWindowViewModel : ViewModelBase
    {
        private readonly DatabaseUnlockedModel database;

        public NewEntryWindowViewModel(DatabaseUnlockedModel database)
        {
            this.database = database;
        }

        private string _payload = "";
        public string Payload
        {
            get => _payload;
            set => SetProperty(ref _payload, value);
        }

        public bool OnOK()
        {
            database.CreateEntry(Payload);
            return true;
        }
    }
}
