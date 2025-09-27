using System;

namespace MKW.GUI.SingleInstance
{
    public class SingleInstanceApplicationManager
    {
        private readonly ISingleInstanceApplication application;

        public SingleInstanceApplicationManager(ISingleInstanceApplication application)
        {
            this.application = application;
        }

        public void Run(string[] args)
        {
            using (Mutex singleInstanceMutex = new Mutex(true,
                                                         SingleInstanceConstants.SingleInstanceMutexName,
                                                         out bool mainInstance))
            {
                if (mainInstance)
                {
                    CancellationTokenSource source = new CancellationTokenSource();

                    using SingleInstanceServer server = new SingleInstanceServer(application);

                    Task serverTask = server.Run(args, source.Token);

                    application.InvokeMainInstance(args);

                    source.Cancel();
                    serverTask.Wait();
                }
                else
                {
                    using SingleInstanceClient client = new SingleInstanceClient();
                    client.Run(args, default).Wait();
                }
            }
        }
    }
}
