namespace MKW.Storage.MKPG
{
    internal sealed record class BlobEntry(
        ReadOnlyMemory<byte> Data
    );
}
