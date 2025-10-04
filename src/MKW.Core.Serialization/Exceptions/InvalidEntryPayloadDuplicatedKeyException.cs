namespace MKW.Core.Serialization.Exceptions
{
    internal sealed class InvalidEntryPayloadDuplicatedKeyException(EntryPayloadKey key)
        : InvalidEntryPayload($"Duplicated key '{key}'.")
    {
    }
}
