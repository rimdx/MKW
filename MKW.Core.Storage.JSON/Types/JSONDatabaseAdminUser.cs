using System.Text.Json.Serialization;

namespace MKW.Core.Storage.JSON.Types
{
    public record class JSONDatabaseAdminUser : JSONDatabaseUser
    {
        // Signed Public Keys of each trusted users by admin's private credentials.
        [JsonRequired]
        public List<ReadOnlyMemory<byte>> Trust { get; set; }

        public JSONDatabaseAdminUser()
        {
            Trust = new List<ReadOnlyMemory<byte>>();
        }
    }
}
