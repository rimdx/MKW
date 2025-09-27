using MKW.GUI.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace MKW.GUI.SingleInstance
{
    internal class MessageService
    {
        private const int SendMessageDefaultTimeout = 2000;

        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

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
                if ((uint)SendMessage(process.MainWindowHandle, SingleInstanceConstants.IdentifyMessageId, IntPtr.Zero) == messageId)
                {
                    COPYDATASTRUCT copyData = new COPYDATASTRUCT
                    {
                        dwData = new IntPtr(messageId),
                        cbData = data.Length * 2, // unicodify
                        lpData = data,
                    };

                    IntPtr copyDataMem = Marshal.AllocHGlobal(Marshal.SizeOf<COPYDATASTRUCT>());

                    Marshal.StructureToPtr(copyData, copyDataMem, false);

                    SendMessage(process.MainWindowHandle, WM.WM_COPYDATA, copyDataMem);

                    Marshal.FreeHGlobal(copyDataMem);

                    return true;
                }
            }

            return false;
        }

        public void AddMessageSource(HwndSource source)
        {
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd,
                               int msg,
                               IntPtr wParam,
                               IntPtr lParam,
                               ref bool handled)
        {
            if (msg == SingleInstanceConstants.IdentifyMessageId)
            {
                handled = true;
                return new IntPtr(SingleInstanceConstants.IdentifyMessageId);
            }
            else if (msg == WM.WM_COPYDATA)
            {
                COPYDATASTRUCT copyData = Marshal.PtrToStructure<COPYDATASTRUCT>(lParam);

                if (MessageReceived != null)
                {
                    MessageReceivedEventArgs args =
                        new MessageReceivedEventArgs((uint)copyData.dwData.ToInt32(),
                                                     copyData.lpData);

                    MessageReceived.Invoke(this, args);
                }

                handled = true;
                return IntPtr.Zero;
            }
            else
            {
                return IntPtr.Zero;
            }
        }
    }
}
