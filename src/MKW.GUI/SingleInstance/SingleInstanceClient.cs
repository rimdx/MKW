using MKW.Common;
using System.IO.Pipes;

namespace MKW.GUI.SingleInstance
{
    internal class SingleInstanceClient : IDisposable
    {
        private readonly NamedPipeClientStream pipe;

        public SingleInstanceClient()
        {
            pipe = new NamedPipeClientStream(SingleInstanceConstants.SingleInstancePipeName);
        }

        public async Task Run(string[] args, CancellationToken cancellationToken)
        {
            await pipe.ConnectAsync(cancellationToken);

            ReadOnlyMemory<byte> buffer = new byte[] { 1, 2, 3 };

            await pipe.WriteAsync(MakePacket(buffer.Span), cancellationToken);
        }

        private ReadOnlyMemory<byte> MakePacket(ReadOnlySpan<byte> buffer)
        {
            ReadOnlyMemory<byte> length = BitConverter.GetBytes(buffer.Length);

            Memory<byte> packet = new byte[buffer.Length + length.Length];

            length.Span.CopyTo(packet.Span.Slice(0));
            buffer.CopyTo(packet.Span.Slice(length.Length));

            return packet;
        }

        public void Dispose()
        {
            pipe.Dispose();
        }
    }
}
