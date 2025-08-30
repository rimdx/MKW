using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class AdminSession : UserSession, IDisposable
    {
        public AdminSession(ClientSession client /* reference */,
                            IDatabase database /* reference */,
                            IDatabaseUser admin /* reference */,
                            ReadOnlySpan<byte> privateKey)
            : base(client, database, admin, privateKey)
        {
        }
    }
}
