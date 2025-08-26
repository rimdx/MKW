using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class AdminSession : UserSession, IDisposable
    {
        public AdminSession(ClientSession client /* reference */,
                            IDatabaseUser admin /* reference */,
                            ReadOnlySpan<byte> privateKey)
            : base(client, admin, privateKey)
        {
        }
    }
}
