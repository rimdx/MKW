namespace MKW.Core.Serialization
{
    internal class BadSequenceLengthException(int count)
        : Exception($"Bad sequence length: {count}")
    {
    }
}
