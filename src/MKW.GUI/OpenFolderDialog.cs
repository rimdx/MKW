using MKW.GUI.Win32;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Interop;

namespace MKW.GUI
{
    public class OpenFolderDialog
    {
        private readonly List<string> resultPaths;
        private readonly List<string> resultNames;

        public IReadOnlyList<string> ResultPaths => resultPaths;
        public IReadOnlyList<string> ResultNames => resultNames;

        public string? ResultPath => ResultPaths.FirstOrDefault();
        public string? ResultName => ResultNames.FirstOrDefault();

        public string? InputPath { get; set; }
        public string? Title { get; set; }
        public string? OkButtonLabel { get; set; }
        public string? FileNameLabel { get; set; }

        // TODO: public bool ForceFileSystem { get; set; }
        // TODO: public bool Multiselect { get; set; }

        public OpenFolderDialog()
        {
            resultPaths = [];
            resultNames = [];
        }

        // for WPF support
        public bool? ShowDialog(Window owner)
        {
            return ShowDialog(new WindowInteropHelper(owner).Handle);
        }

        // for all .NET
        public bool? ShowDialog(IntPtr owner)
        {
            IFileOpenDialog dialog = (IFileOpenDialog)new FileOpenDialog();

            if (InputPath != null)
            {
                Marshal.ThrowExceptionForHR(Shell32.SHCreateItemFromParsingName(InputPath,
                                                                                null,
                                                                                typeof(IShellItem).GUID,
                                                                                out IShellItem item));

                dialog.SetFolder(item);
            }

            dialog.SetOptions(FOS.FOS_PICKFOLDERS);

            if (Title != null)
            {
                dialog.SetTitle(Title);
            }

            if (OkButtonLabel != null)
            {
                dialog.SetOkButtonLabel(OkButtonLabel);
            }

            if (FileNameLabel != null)
            {
                dialog.SetFileName(FileNameLabel);
            }

            int hr = dialog.Show(owner);

            if (hr == ErrorCodes.ERROR_CANCELLED)
            {
                return null;
            }
            else
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            Marshal.ThrowExceptionForHR(dialog.GetResults(out IShellItemArray? items));

            items.GetCount(out int count);
            for (int i = 0; i < count; i++)
            {
                items.GetItemAt(i, out IShellItem? item);

                Marshal.ThrowExceptionForHR(item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out string? path));
                Marshal.ThrowExceptionForHR(item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEEDITING, out string? name));

                if (path != null && name != null)
                {
                    resultPaths.Add(path);
                    resultNames.Add(name);
                }
            }
            return true;
        }
    }
}
