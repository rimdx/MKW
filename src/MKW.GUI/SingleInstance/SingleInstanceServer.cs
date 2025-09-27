using MKW.Common;
using System.IO.Pipes;

namespace MKW.GUI.SingleInstance
{
    internal class SingleInstanceServer : IDisposable
    {
        private readonly NamedPipeServerStream pipe;
        private readonly ISingleInstanceApplication application;

        public SingleInstanceServer(ISingleInstanceApplication application)
        {
            this.application = application;
            pipe = new NamedPipeServerStream(SingleInstanceConstants.SingleInstancePipeName);
        }

        public async Task Run(string[] args, CancellationToken cancellationToken)
        {
            try
            {
                await pipe.WaitForConnectionAsync(cancellationToken);

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    ReadOnlyMemory<byte> data = await ReadPacket(cancellationToken);

                    application.InvokeExternalInstance([]);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        private async Task<ReadOnlyMemory<byte>> ReadPacket(CancellationToken cancellationToken)
        {
            Memory<byte> lengthBuffer = new byte[4];
            await pipe.ReadAsync(lengthBuffer, cancellationToken);

            int length = BitConverter.ToInt32(lengthBuffer.ToArray(), 0);
            Memory<byte> dataBuffer = new byte[lengthBuffer.Length];

            await pipe.ReadAsync(dataBuffer, cancellationToken);

            return dataBuffer;
        }

        public void Dispose()
        {
            pipe.Dispose();
        }
    }
}
