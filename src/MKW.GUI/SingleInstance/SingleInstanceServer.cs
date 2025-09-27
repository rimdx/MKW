using MKW.Common;
using System.IO.Pipes;

namespace MKW.GUI.SingleInstance
{
    internal class SingleInstanceServer : IDisposable
    {
        private readonly NamedPipeServerStream pipe;
        private readonly PacketRPC rpc;
        private readonly ISingleInstanceApplication application;

        public SingleInstanceServer(ISingleInstanceApplication application)
        {
            this.application = application;
            pipe = new NamedPipeServerStream(SingleInstanceConstants.SingleInstancePipeName);
            rpc = new PacketRPC(pipe);
        }

        public async Task Run(string[] args, CancellationToken cancellationToken)
        {
            try
            {
                await pipe.WaitForConnectionAsync(cancellationToken);

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    ReadOnlyMemory<byte> data = await rpc.ReadPacket(cancellationToken);
                    RunRequest request = RunRequestSerializer.Deserialize(data.Span);

                    application.InvokeExternalInstance(request.Args);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        public void Dispose()
        {
            pipe.Dispose();
        }
    }
}
