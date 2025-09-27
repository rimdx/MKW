using System.IO.Pipes;

namespace MKW.GUI.SingleInstance
{
    public class SingleInstanceApplicationManager
    {
        private static readonly string SingleInstanceMutexName = "{D22C3E6A-1B2F-4B80-9F54-A601E5CBEC37}";
        private static readonly string SingleInstancePipeName = "{09FCE022-F082-45F0-9E3C-92556F6C6079}";

        private readonly ISingleInstanceApplication application;

        public SingleInstanceApplicationManager(ISingleInstanceApplication application)
        {
            this.application = application;
        }

        public void Run(string[] args)
        {
            using (Mutex singleInstanceMutex = new Mutex(true,
                                                         SingleInstanceMutexName,
                                                         out bool mainInstance))
            {
                if (mainInstance)
                {
                    application.InvokeMainInstance(args);
                }
                else
                {
                    // signal main app
                }
            }
        }
    }
}
