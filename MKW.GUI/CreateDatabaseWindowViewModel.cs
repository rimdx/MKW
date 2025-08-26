using MKW.GUI.Model;

namespace MKW.GUI
{
    public class CreateDatabaseWindowViewModel : ViewModelBase
    {
        private readonly string path;

        public DatabaseModel? Database { get; private set; }

        public CreateDatabaseWindowViewModel(string path)
        {
            this.path = path;
        }

        public string Password { get; set; } = "";
        public string DatabasePath => path;

        public bool DoCreateDatabase() => RunAction(() =>
        {
            Database = DatabaseModel.Create(path, Password);
        });
    }
}
