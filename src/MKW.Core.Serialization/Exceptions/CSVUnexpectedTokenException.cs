namespace MKW.Core.Serialization.Exceptions
{
    public sealed class CSVUnexpectedTokenException(Type actual)
        : Exception($"Unexpected CSV token: {actual.Name}")
    {
    }
}
