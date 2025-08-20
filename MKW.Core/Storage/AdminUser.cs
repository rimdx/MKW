namespace MKW.Core.Storage
{
    public record class AdminUser : DatabaseUser
    {
        // Signed Public Keys of each trusted users by admin's private credentials.
        // FIXME: comparable signature instead of byte[]!!!
        public HashSet<Memory<byte>> Trust { get; set; }

        public AdminUser()
        {
            Trust = new HashSet<Memory<byte>>();
        }
    }
}
