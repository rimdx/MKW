namespace MKW.Core.Exceptions
{
    [Serializable]
    public class InvalidUserMetadataSignature()
        : Exception("Metadata signature is not valid.")
    {
    }
}
