namespace MKW.Core.Storage
{
    public interface IDatabaseUser : ISavable
    {
        Guid Id { get; }
        Memory<byte> Salt { get; set; }
        Memory<byte> PublicKey { get; set; }
        Memory<byte> PrivateKey { get; set; }
    }
}
