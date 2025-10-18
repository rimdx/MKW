
namespace MKW.Storage.MKPG
{
    internal interface IBlobStorage
    {
        void Create(BlobEntry entry);

        BlobEntry Open(BlobId id);

        bool Delete(BlobId id);

        IEnumerable<BlobEntry> Enumerate();

        bool Exists(BlobId id);
    }
}