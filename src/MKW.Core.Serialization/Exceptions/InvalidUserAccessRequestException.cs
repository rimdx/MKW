namespace MKW.Core.Serialization.Exceptions
{
    public sealed class InvalidUserAccessRequestException(Exception innerException)
        : Exception("User access request is invalid.", innerException)
    {
    }
}
