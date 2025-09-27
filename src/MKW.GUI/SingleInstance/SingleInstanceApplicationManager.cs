using System.Diagnostics;
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
            messageService.MessageReceived += MessageReceived;
        }

        public bool Run(RunRequest request)
        {
            using (Mutex singleInstanceMutex = new Mutex(true, SingleInstanceConstants.SingleInstanceMutexName))
            {
                string encoded = RunRequestSerializer.Serialize(request);

                IReadOnlyCollection<IOtherAppWindow> windows = messageService.GetOtherAppWindows();
                foreach (IOtherAppWindow window in windows)
                {
                    window.SendDataMessage(SingleInstanceConstants.IdentifyMessageId, encoded);

                    // messages broadcasted successfully -> no new host required
                    return false;
                }

                return true;
            }
        }

        public void AddMessageSource(HwndSource source)
        {
            messageService.AddMessageSource(source);
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                RunRequest request = RunRequestSerializer.Deserialize(e.Data);

                application.InvokeExternalInstance(request);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
            }
        }
    }
}
