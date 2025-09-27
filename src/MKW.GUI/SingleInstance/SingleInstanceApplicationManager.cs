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
            // todo:
            application.InvokeMainInstance(args);
        }
    }
}
