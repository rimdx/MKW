using MKW.Core.Client;
using MKW.Core.Storage;
using System.ComponentModel;

namespace MKW.GUI
{
    public class DatabaseModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IDatabase database;
        private readonly ClientSession client;

        public event PropertyChangedEventHandler? PropertyChanged;

        public DatabaseModel(IDatabase database)
        {
            this.database = database;

            client = ClientSession.Open(database);
        }

        public void Dispose()
        {
            database.Dispose();
        }
    }
}
