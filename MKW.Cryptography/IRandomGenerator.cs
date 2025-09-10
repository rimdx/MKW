namespace MKW.Cryptography
{
    public interface IRandomGenerator
    {
        byte[] NextBytes(int length);
    }
}
