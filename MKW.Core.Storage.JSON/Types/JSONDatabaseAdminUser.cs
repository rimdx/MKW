namespace MKW.Core.Storage.JSON.Types
{
    public record class JSONDatabaseAdminUser : JSONDatabaseUser
    {
        // Signed Public Keys of each trusted users by admin's private credentials.
        public required List<ReadOnlyMemory<byte>> Trust { get; set; }
    }
}
