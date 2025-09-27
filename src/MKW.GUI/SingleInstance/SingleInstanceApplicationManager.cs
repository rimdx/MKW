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
                if (messageService.BroadcastMessage(SingleInstanceConstants.OpenFileMessageId, "123"))
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
            application.InvokeExternalInstance([]);
        }
    }
}
