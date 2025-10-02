namespace MKW.Core.Serialization.Exceptions
{
    public sealed class InvalidUserMetadataException(Exception innerException)
        : Exception("User metadata is invalid.", innerException)
    {
    }
}
