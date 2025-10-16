
namespace MKW.Storage.MKPG
{
    internal interface IBlobStorage
    {
        void Create(BlobId id, BlobEntry entry);

        BlobEntry Open(BlobId id);

        bool Delete(BlobId id);

        IEnumerable<BlobId> Enumerate();

        bool Exists(BlobId id);
    }
}