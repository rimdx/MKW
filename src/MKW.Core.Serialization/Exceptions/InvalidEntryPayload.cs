namespace MKW.Core.Serialization.Exceptions
{
    public class InvalidEntryPayload : Exception
    {
        protected InvalidEntryPayload(string reason)
            : base($"Entry payload is invalid: {reason}")
        {
        }

        internal InvalidEntryPayload(Exception ex)
            : base($"Entry payload is invalid.", ex)
        {
        }
    }
}
