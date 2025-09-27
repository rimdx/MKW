using MKW.GUI.Win32;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace MKW.GUI.SingleInstance
{
    public class SingleInstanceApplicationManager
    {
        private readonly ISingleInstanceApplication application;
        private readonly MessageService messageService;

        public SingleInstanceApplicationManager(ISingleInstanceApplication application)
        {
            this.application = application;
            messageService = new MessageService();
        }

        public void Run(string[] args)
        {
            using (Mutex singleInstanceMutex = new Mutex(true, SingleInstanceConstants.SingleInstanceMutexName))
            {
                if (!messageService.BroadcastMessage(SingleInstanceConstants.OpenFileMessageId, "123"))
                {
                    application.InvokeMainInstance(args);
                }
            }
        }

        public void RunServer(HwndSource source)
        {
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd,
                               int msg,
                               IntPtr wParam,
                               IntPtr lParam,
                               ref bool handled)
        {
            if (msg == SingleInstanceConstants.OpenFileMessageId)
            {
                handled = true;
                return new IntPtr(SingleInstanceConstants.OpenFileMessageId);
            }
            else if (msg == (uint)WM.WM_COPYDATA)
            {
                COPYDATASTRUCT copyData = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));

                application.InvokeExternalInstance([]);

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
