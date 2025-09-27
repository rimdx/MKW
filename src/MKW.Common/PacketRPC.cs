namespace MKW.Common
{
    public class PacketRPC : IDisposable
    {
        private readonly Stream proxy;

        public PacketRPC(Stream proxy)
        {
            this.proxy = proxy;
        }

        public async Task<ReadOnlyMemory<byte>> ReadPacket(CancellationToken cancellationToken)
        {
            Memory<byte> lengthBuffer = new byte[4];
            await proxy.ReadAsync(lengthBuffer, cancellationToken);

            int length = BitConverter.ToInt32(lengthBuffer.ToArray(), 0);
            Memory<byte> dataBuffer = new byte[length];

            await proxy.ReadAsync(dataBuffer, cancellationToken);

            return dataBuffer;
        }

        public async Task WritePacketAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            ReadOnlyMemory<byte> length = BitConverter.GetBytes(buffer.Length);

            Memory<byte> packet = new byte[buffer.Length + length.Length];

            length.Span.CopyTo(packet.Span.Slice(0));
            buffer.Span.CopyTo(packet.Span.Slice(length.Length));

            await proxy.WriteAsync(packet, cancellationToken);
        }

        public void Dispose()
        {
            proxy.Dispose();
        }
    }
}
