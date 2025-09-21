namespace MKW.Core.Client.Exceptions
{
    [Serializable]
    public class InvalidUserMetadataSignature()
        : Exception("Metadata signature is not valid.")
    {
    }
}
