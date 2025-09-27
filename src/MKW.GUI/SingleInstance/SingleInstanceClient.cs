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

            ReadOnlyMemory<byte> buffer = new byte[] { 1, 2, 3 };

            await rpc.WritePacketAsync(buffer, cancellationToken);
        }

        public void Dispose()
        {
            pipe.Dispose();
        }
    }
}
