using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Editor
{
    public class TrustEditor
        : ITrustProvider
        , ITrustController
        , IDisposable
    {
        private readonly IUserSession proxy;
        private readonly Dictionary<UserId, Trust> edits;

        public TrustEditor(IUserSession proxy /* todo: better type */)
        {
            this.proxy = proxy;
            edits = [];
        }

        public void UpdateTrust(UserId userId, Trust trust)
        {
            edits[userId] = trust;
        }

        public IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsers()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers()
        {
            throw new NotImplementedException();
        }

        public Trust GetExplicitTrust(ReadOnlySpan<byte> publicKey)
        {
            throw new NotImplementedException();
        }

        public Trust GetImplicitTrust(UserId userId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            proxy.Dispose();
        }

        public void Commit()
        {
            foreach (KeyValuePair<UserId, Trust> edit in edits)
            {
                proxy.UpdateTrust(edit.Key, edit.Value);
            }
        }
    }
}
