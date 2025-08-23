using MKW.Core.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MKW.GUI
{
    public class DatabaseModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ClientSession client;
        private readonly UserSession user;

        public event PropertyChangedEventHandler? PropertyChanged;

        public DatabaseModel(ClientSession client, UserSession user)
        {
            this.client = client;
            this.user = user;

            Entries = [];

            foreach (UserEntry entry in user.EnumerateEntries())
            {
                Entries.Add(new DatabaseEntryModel
                {
                    Id = entry.Id,
                    Payload = entry.OpenPayload()?.ToString()
                });
            }
        }

        public ObservableCollection<DatabaseEntryModel> Entries { get; private set; }

        public void Dispose()
        {
            client.Dispose();
            user.Dispose();
        }
    }
}
