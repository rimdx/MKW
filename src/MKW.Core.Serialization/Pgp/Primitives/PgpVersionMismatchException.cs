namespace MKW.Core.Serialization.Pgp.Primitives
{
    public sealed class PgpVersionMismatchException(int expected, int actual)
        : Exception($"PGP version mismatch: expected {expected} but was {actual}.")
    {
    }
}
