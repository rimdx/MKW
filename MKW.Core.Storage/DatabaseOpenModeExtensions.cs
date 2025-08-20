namespace MKW.Core.Storage
{
    public static class DatabaseOpenModeExtensions
    {
        public static FileMode GetNativeFileMode(this DatabaseOpenMode mode) => mode switch
        {
            DatabaseOpenMode.ReadOnly => FileMode.Open,
            DatabaseOpenMode.Open => FileMode.Open,
            DatabaseOpenMode.OpenOrCreate => FileMode.OpenOrCreate,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), "Invalid database open mode."),
        };

        public static FileAccess GetNativeFileAccess(this DatabaseOpenMode mode) => mode switch
        {
            DatabaseOpenMode.ReadOnly => FileAccess.Read,
            DatabaseOpenMode.Open => FileAccess.ReadWrite,
            DatabaseOpenMode.OpenOrCreate => FileAccess.ReadWrite,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), "Invalid database open mode."),
        };
    }
}
