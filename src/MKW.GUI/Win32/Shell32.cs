using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace MKW.GUI.Win32
{
    public static class Shell32
    {
        [DllImport("shell32")]
        public static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath,
                                                             IBindCtx? pbc,
                                                             [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                                                             out IShellItem ppv);
    }
}
