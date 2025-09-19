namespace MKW.Core
{
    public interface IAccessRequestSerializer
    {
        UserAccessRequest Deserialize(ReadOnlySpan<byte> data);
        ReadOnlyMemory<byte> Serialize(UserAccessRequest data);
    }
}
