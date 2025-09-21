using Microsoft.Win32;

namespace MKW.GUI.Services
{
    public class RegistryService : IDisposable
    {
        private readonly RegistryKey rootKey;

        public RegistryService(string rootKeyPath)
        {
            rootKey = Registry.CurrentUser.CreateSubKey(rootKeyPath);
        }

        public string[] GetRecentFiles()
        {
            return GetValue<string[]>(RegistryKeys.RecentFilesKeyName, []);
        }

        public void SetRecentFiles(string[] recentFiles)
        {
            SetValue(RegistryKeys.RecentFilesKeyName, recentFiles);
        }

        public string GetLastDatabaseDirectory()
        {
            string docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return GetValue(RegistryKeys.LastDatabaseDirectoryKeyName, docsPath);
        }

        public void SetLastDatabaseDirectory(string path)
        {
            SetValue(RegistryKeys.LastDatabaseDirectoryKeyName, path);
        }

        public string[] GetOpenFiles()
        {
            return GetValue<string[]>(RegistryKeys.OpenFilesKeyName, []);
        }

        public void SetOpenFiles(string[] recentFiles)
        {
            SetValue(RegistryKeys.OpenFilesKeyName, recentFiles);
        }

        private T GetValue<T>(string key, T defaultValue) where T : class
        {
            object? value = rootKey.GetValue(key);

            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }
                else
                {
                    return defaultValue;
                }
            }
        }

        private void SetValue<T>(string key, T value) where T : class
        {
            rootKey.SetValue(key, value);
        }

        public void Dispose()
        {
            rootKey.Dispose();
        }
    }
}
