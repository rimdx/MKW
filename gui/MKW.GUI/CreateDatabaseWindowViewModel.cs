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

        public string Password { get; set; }
        public bool IsPasswordMatch { get; set; }
        public string DatabasePath => path;

        public bool DoCreateDatabase() => RunAction(() =>
        {
            if (!IsPasswordMatch)
            {
                throw new Exception("Password and repeated password don't match.");
            }

            if (Password == null)
            {
                throw new ArgumentNullException(nameof(Password));
            }

            Database = DatabaseModel.Create(path, Password);
        });
    }
}
