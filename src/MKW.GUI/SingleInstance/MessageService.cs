using MKW.GUI.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MKW.GUI.SingleInstance
{
    internal class MessageService
    {
        private const int SendMessageDefaultTimeout = 2000;

        public MessageService()
        {
        }

        public IntPtr SendMessage(IntPtr windowHandle, uint messageId, IntPtr data)
        {
            IntPtr hr = User32.SendMessageTimeout(windowHandle,
                                                  messageId,
                                                  IntPtr.Zero,
                                                  data,
                                                  SendMessageTimeoutFlags.SMTO_BLOCK | SendMessageTimeoutFlags.SMTO_ABORTIFHUNG,
                                                  SendMessageDefaultTimeout,
                                                  out IntPtr result);

            if (hr == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            else
            {
                return result;
            }
        }

        public bool BroadcastMessage(uint messageId, string data)
        {
            foreach (Process process in Process.GetProcesses())
            {
                if ((uint)SendMessage(process.MainWindowHandle, messageId, IntPtr.Zero) == messageId)
                {
                    COPYDATASTRUCT copyData = new COPYDATASTRUCT
                    {
                        dwData = IntPtr.Zero,
                        cbData = data.Length * 2, // unicodify
                        lpData = data,
                    };

                    IntPtr copyDataMem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(COPYDATASTRUCT)));

                    Marshal.StructureToPtr(copyData, copyDataMem, false);

                    SendMessage(process.MainWindowHandle, (uint)WM.WM_COPYDATA, copyDataMem);

                    Marshal.FreeHGlobal(copyDataMem);

                    return true;
                }
            }

            return false;
        }
    }
}
