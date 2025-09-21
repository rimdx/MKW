namespace MKW.GUI.Database
{
    public class DatabaseLockedViewModel : ViewModelBase
    {
        public DatabaseViewModel Database { get; }

        public DatabaseLockedViewModel(DatabaseViewModel database)
        {
            Database = database;
        }
    }
}
