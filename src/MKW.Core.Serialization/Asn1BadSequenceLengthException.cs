namespace MKW.Core.Serialization
{
    internal sealed class Asn1BadSequenceLengthException(int count)
        : Exception($"Bad sequence length: {count}")
    {
    }
}
