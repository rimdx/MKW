using System.ComponentModel;

namespace MKW.GUI
{
    public class CreateDatabaseWindowViewModel : INotifyPropertyChanged
    {
        private readonly string path;

        public DatabaseModel? Database { get; private set; }

        public CreateDatabaseWindowViewModel(string path)
        {
            this.path = path;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Password { get; set; } = "";
        public string DatabasePath => path;

        public bool DoCreateDatabase()
        {
            Database = DatabaseModel.Create(path, Password);
            return true;
        }
    }
}
