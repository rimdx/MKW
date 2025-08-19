namespace MKW.Core.Storage
{
    public record class AdminUser : DatabaseUser
    {
        // Signed Public Keys of each trusted users by admin's private credentials.
        public required IList<byte[]> Trust { get; set; }

        public AdminUser()
        {
            Trust = new List<byte[]>();
        }
    }
}
