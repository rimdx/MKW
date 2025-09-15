using MKW.Core.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Editor
{
    public class TrustEditor
        : ITrustProvider
        , ITrustController
        , IDisposable
    {
        private enum EditAction
        {
            Add,
            Delete,
        }

        private readonly IUserSession proxy;
        private readonly Dictionary<UserId, EditAction> edits;

        public TrustEditor(IUserSession proxy /* todo: better type */)
        {
            this.proxy = proxy;
            edits = [];
        }

        public void AddTrust(UserId userId)
        {
            edits[userId] = EditAction.Add;
        }

        public void RemoveTrust(UserId userId)
        {
            edits[userId] = EditAction.Delete;
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
            foreach (KeyValuePair<UserId, EditAction> edit in edits)
            {
                switch (edit.Value)
                {
                    case EditAction.Add:
                        proxy.AddTrust(edit.Key);
                        break;
                    case EditAction.Delete:
                        proxy.RemoveTrust(edit.Key);
                        break;
                }
            }
        }
    }
}
