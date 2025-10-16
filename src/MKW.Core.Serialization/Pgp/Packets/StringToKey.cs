namespace MKW.Core.Serialization.Pgp.Packets
{
    public abstract record class StringToKey
    {
        public required StringToKeyTag Tag { get; init; }
    }
}
