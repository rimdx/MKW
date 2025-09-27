using System.Text.Json;

namespace MKW.GUI.SingleInstance
{
    internal static class RunRequestSerializer
    {
        public static byte[] Serialize(RunRequest data)
        {
            return JsonSerializer.SerializeToUtf8Bytes(
                data, RunRequestSerializerContext.Default.RunRequest);
        }

        public static RunRequest Deserialize(byte[] data)
        {
            RunRequest? parsed = JsonSerializer.Deserialize(
                data, RunRequestSerializerContext.Default.RunRequest);

            if (parsed == null)
            {
                throw new NullReferenceException();
            }

            return parsed;
        }
    }
}
