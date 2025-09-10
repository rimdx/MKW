namespace MKW.Core.Cryptography
{
    public interface IRandomGenerator
    {
        byte[] NextBytes(int length);
    }
}
