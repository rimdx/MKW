using MKW.GUI.Model;
using System.IO;

namespace MKW.GUI
{
    public class CreateDatabaseWindowViewModel : ViewModelBase
    {
        public DatabaseModel? Database { get; private set; }

        public CreateDatabaseWindowViewModel()
        {
            // TODO: factor out
            // TODO: save latest directory to registry storage
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            m_databasePath = Path.Combine(documentsPath, "New Database.mkw");
        }

        public string Password { get; set; }
        public bool IsPasswordMatch { get; set; }

        private string m_databasePath;
        public string DatabasePath
        {
            get => m_databasePath;
            set => SetProperty(ref m_databasePath, value);
        }

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

            Database = DatabaseModel.Create(DatabasePath, Password);
        });
    }
}
