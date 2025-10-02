namespace MKW.Core.Serialization
{
    internal sealed class Asn1VersionMismatch(int expected, int actual)
        : Exception($"Asn1 version mismatch: expected {expected} but was {actual}.")
    {
    }
}
