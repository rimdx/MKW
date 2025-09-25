using MKW.GUI.Model;

namespace MKW.GUI
{
    public class NewEntryWindowViewModel : ViewModelBase
    {
        private readonly DatabaseModel database;

        public NewEntryWindowViewModel(DatabaseModel database)
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
