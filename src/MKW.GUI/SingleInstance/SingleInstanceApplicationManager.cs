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

        public bool Run(string[] args)
        {
            using (Mutex singleInstanceMutex = new Mutex(true, SingleInstanceConstants.SingleInstanceMutexName))
            {
                string encoded = RunRequestSerializer.Serialize(new RunRequest(args));

                if (messageService.BroadcastMessage(SingleInstanceConstants.OpenFileMessageId, encoded))
                {
                    // messages broadcasted successfully -> no new host required
                    return false;
                }
                else
                {
                    return true;
                }
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
                RunRequest decoded = RunRequestSerializer.Deserialize(e.Data);

                application.InvokeExternalInstance(decoded.Args);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
            }
        }
    }
}
