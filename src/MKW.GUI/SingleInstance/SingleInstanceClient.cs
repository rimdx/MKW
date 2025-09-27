using MKW.Common;
using System.IO.Pipes;

namespace MKW.GUI.SingleInstance
{
    internal class SingleInstanceClient : IDisposable
    {
        private readonly NamedPipeClientStream pipe;
        private readonly PacketRPC rpc;

        public SingleInstanceClient()
        {
            pipe = new NamedPipeClientStream(SingleInstanceConstants.SingleInstancePipeName);
            rpc = new PacketRPC(pipe);
        }

        public async Task Run(string[] args, CancellationToken cancellationToken)
        {
            await pipe.ConnectAsync(cancellationToken);

            RunRequest request = new RunRequest(args);
            ReadOnlyMemory<byte> buffer = RunRequestSerializer.Serialize(request);

            await rpc.WritePacketAsync(buffer, cancellationToken);
        }

        public void Dispose()
        {
            pipe.Dispose();
        }
    }
}
