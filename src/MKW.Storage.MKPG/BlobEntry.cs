namespace MKW.Storage.MKPG
{
    internal sealed record class BlobEntry(
        BlobId Id,
        ReadOnlyMemory<byte> Data
    );
}
