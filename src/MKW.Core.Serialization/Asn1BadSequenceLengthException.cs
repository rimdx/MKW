namespace MKW.Core.Serialization
{
    internal class Asn1BadSequenceLengthException(int count)
        : Exception($"Bad sequence length: {count}")
    {
    }
}
