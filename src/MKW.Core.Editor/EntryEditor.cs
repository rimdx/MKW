using MKW.Core.Storage;

namespace MKW.Core.Editor
{
    public class EntryEditor : IEntrySession, IDisposable
    {
        private readonly IEntrySession proxy;

        public EntryId Id => proxy.Id;

        private EntryPayload? payloadBefore;
        private EntryPayload? payloadAfter;

        public EntryEditor(IEntrySession proxy /* move */)
        {
            this.proxy = proxy;

            payloadBefore = proxy.OpenPayload();
            payloadAfter = payloadBefore;
        }

        public IEnumerable<UserInfo> EnumerateAccess()
        {
            foreach (UserInfo user in EnumerateAccess())
            {
                yield return user;
            }
        }

        public EntryPayload? OpenPayload()
        {
            return payloadAfter;
        }

        public EntryInfo UpdatePayload(EntryPayload payload)
        {
            payloadAfter = payload;

            return new EntryInfo
            {
                Id = proxy.Id,
                EncodedForUsers = [] // todo:
            };
        }

        public void AddAccess(UserId userId)
        {
            proxy.AddAccess(userId);
        }

        public void Commit()
        {
            if (payloadBefore != payloadAfter && payloadAfter != null)
            {
                proxy.UpdatePayload(payloadAfter);
                payloadBefore = payloadAfter;
            }
        }

        public void Dispose()
        {
            proxy.Dispose();
        }
    }
}
