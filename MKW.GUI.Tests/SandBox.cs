using Microsoft.Win32;

namespace MKW.GUI.Tests
{
    public class SandBox : IDisposable
    {
        public static readonly string TestRootKey =
            Path.Combine(RegistryKeys.RootKeyPath, "UnitTesting");

        public RegistryService RegistryService;

        public SandBox()
        {
            Registry.CurrentUser.DeleteSubKey(TestRootKey);
            RegistryService = new RegistryService(TestRootKey);
        }

        public void Dispose()
        {
            RegistryService.Dispose();
        }
    }
}
