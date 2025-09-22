namespace MKW.Core
{
    public interface IUserSession
        : IEntryController
        , ITrustProvider
        , IDisposable
    {
        UserId Id { get; }

        UserMetadata OpenMetadata();
    }
}
