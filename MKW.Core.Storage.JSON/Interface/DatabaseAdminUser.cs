namespace MKW.Core.Storage.JSON.Interface
{
    internal record class DatabaseAdminUser : DatabaseUser, IDatabaseUser, ISavable
    {
        internal DatabaseAdminUser(MemoryDatabaseSession host)
            : base(UserId.Admin(), host)
        {
        }

        public DatabaseAdminUser()
            : base(UserId.Admin())
        {
        }

        public override void Save()
        {
            if (host == null)
            {
                throw new InvalidOperationException();
            }

            host.Database.Admin = AsJSONObject();
            host.Save();
        }
    }
}
