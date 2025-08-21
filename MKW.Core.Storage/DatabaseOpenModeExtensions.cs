namespace MKW.Core.Storage
{
    public static class DatabaseOpenModeExtensions
    {
        public static FileMode GetNativeFileMode(this DatabaseOpenMode mode) => mode switch
        {
            DatabaseOpenMode.ReadOnly => FileMode.Open,
            DatabaseOpenMode.Open => FileMode.Open,
            DatabaseOpenMode.OpenOrCreate => FileMode.OpenOrCreate,
        };

        public static FileAccess GetNativeFileAccess(this DatabaseOpenMode mode) => mode switch
        {
            DatabaseOpenMode.ReadOnly => FileAccess.Read,
            DatabaseOpenMode.Open => FileAccess.ReadWrite,
            DatabaseOpenMode.OpenOrCreate => FileAccess.ReadWrite,
        };
    }
}
