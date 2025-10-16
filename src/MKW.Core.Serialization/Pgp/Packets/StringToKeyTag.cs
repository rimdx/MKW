namespace MKW.Core.Serialization.Pgp.Packets
{
    public enum StringToKeyTag : byte
    {
        Simple = 0,
        Salted = 1,
        IteratedSalted = 3,
    }
}
