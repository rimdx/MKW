using System.Buffers;

namespace MKW.Core.Serialization.Pgp
{
    public static class BuffersExtensions
    {
        public static void Write<T>(this IBufferWriter<T> writer,
                                    T value)
        {
            Span<T> span = [
                value,
            ];

            writer.Write(span);
        }
    }
}
