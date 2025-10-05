namespace MKW.Core.Exceptions
{
    public sealed class InvalidEntryPayloadKey(string reason)
        : Exception($"Invalid entry payload key: {reason}")
    {
    }
}
