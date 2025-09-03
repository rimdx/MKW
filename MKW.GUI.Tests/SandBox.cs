using Microsoft.Win32;
using MKW.Testing.Common;

namespace MKW.GUI.Tests
{
    public class SandBox : SandBoxBase
    {
        public RegistryService RegistryService;

        public SandBox()
        {
            RegistryService = new RegistryService(TestRootKey);
        }

        public override void Dispose()
        {
            RegistryService.Dispose();
            Registry.CurrentUser.DeleteSubKey(TestRootKey, false);
        }
    }
}
