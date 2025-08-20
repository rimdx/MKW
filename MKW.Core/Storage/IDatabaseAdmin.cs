namespace MKW.Core.Storage
{
    public interface IDatabaseAdmin : IDatabaseUser, ISavable
    {
        HashSet<Memory<byte>> Trust { get; set; }
    }
}
