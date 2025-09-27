using MKW.GUI.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace MKW.GUI.SingleInstance
{
    internal class MessageService
    {
        private class OtherAppWindow : IOtherAppWindow
        {
            private readonly IntPtr hwnd;

            public OtherAppWindow(IntPtr hwnd)
            {
                this.hwnd = hwnd;
            }

            public bool SendDataMessage(uint messageId, byte[] data)
            {
                IntPtr dataMem = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, dataMem, data.Length);

                COPYDATASTRUCT copyData = new COPYDATASTRUCT
                {
                    dwData = new IntPtr(messageId),
                    cbData = data.Length,
                    lpData = dataMem,
                };

                IntPtr copyDataMem = Marshal.AllocHGlobal(Marshal.SizeOf<COPYDATASTRUCT>());

                Marshal.StructureToPtr(copyData, copyDataMem, false);

                SendMessage(hwnd, WM.WM_COPYDATA, copyDataMem);

                Marshal.FreeHGlobal(copyDataMem);
                Marshal.FreeHGlobal(dataMem);

                return true;
            }
        }

        private const int SendMessageDefaultTimeout = 2000;
        // Use uint to support 32-bit platoforms.
        private const uint IdentifyMessageMagicReturnValue = 0x018d7d2e;

        private readonly uint identifyMessageId;

        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

        public MessageService(string applicationMagic)
        {
            string identifyMessageName = $"{applicationMagic}.IdentifyMessage";
            identifyMessageId = User32.RegisterWindowMessage(identifyMessageName);
        }

        private static IntPtr SendMessage(IntPtr windowHandle, uint messageId, IntPtr data)
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

        public IReadOnlyCollection<IOtherAppWindow> GetOtherAppWindows()
        {
            List<IOtherAppWindow> result = new List<IOtherAppWindow>();

            foreach (Process process in Process.GetProcesses())
            {
                IntPtr mainWindowHandle = process.MainWindowHandle;

                IntPtr rv = SendMessage(mainWindowHandle, identifyMessageId, IntPtr.Zero);
                if (rv == new IntPtr(IdentifyMessageMagicReturnValue))
                {
                    result.Add(new OtherAppWindow(mainWindowHandle));
                }
            }

            return result;
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
            if (msg == identifyMessageId)
            {
                handled = true;
                return new IntPtr(IdentifyMessageMagicReturnValue);
            }
            else if (msg == WM.WM_COPYDATA)
            {
                COPYDATASTRUCT copyData = Marshal.PtrToStructure<COPYDATASTRUCT>(lParam);

                byte[] data = new byte[copyData.cbData];
                Marshal.Copy(copyData.lpData, data, 0, copyData.cbData);

                if (MessageReceived != null)
                {
                    MessageReceivedEventArgs args =
                        new MessageReceivedEventArgs((uint)copyData.dwData.ToInt32(),
                                                     data);

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
