using System.Text.Json;

namespace MKW.GUI.SingleInstance
{
    internal static class RunRequestSerializer
    {
        public static string Serialize(RunRequest data)
        {
            return JsonSerializer.Serialize(
                data, RunRequestSerializerContext.Default.RunRequest);
        }

        public static RunRequest Deserialize(string data)
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
