namespace MKW.Core.Serialization.Pgp
{
    public abstract class PgpObject
    {
        public abstract void Encode(PgpOutputStream stream);
    }
}
