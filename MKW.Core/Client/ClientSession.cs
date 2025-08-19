using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        private readonly IDatabase db;

        public ClientSession(IDatabase db)
        {
            this.db = db;
        }

        public void Dispose()
        {
        }
    }
}
