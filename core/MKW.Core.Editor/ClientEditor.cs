using MKW.Core.Client;

namespace MKW.Core.Editor
{
    public class ClientEditor : IDisposable
    {
        private readonly ClientSession client;

        public ClientEditor(ClientSession client)
        {
            this.client = client;
        }

        public void Dispose()
        {
        }
    }
}
