using Microsoft.Win32;

namespace MKW.GUI
{
    public class RegistryService : IDisposable
    {
        private RegistryKey rootKey;

        public RegistryService()
        {
            rootKey = Registry.CurrentUser.CreateSubKey(RegistryKeys.RootKeyPath);
        }

        public string[] GetRecentFiles()
        {
            return GetValue<string[]>(RegistryKeys.RecentFilesKeyName, []);
        }

        public void SetRecentFiles(string[] recentFiles)
        {
            SetValue(RegistryKeys.RecentFilesKeyName, recentFiles);
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
