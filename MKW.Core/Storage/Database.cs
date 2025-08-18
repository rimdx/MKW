namespace MKW.Core.Storage
{
    public class Database
    {
        public IList<User> Users { get; set; }

        public Database()
        {
            Users = new List<User>();
        }
    }
}
