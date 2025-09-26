namespace MKW.Common
{
    public static class TaskExtensions
    {
        public static async Task<T> WaitAsync<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            async Task<T> canceller()
            {
                await Task.Delay(-1, cancellationToken);
                // never reached
                throw new TaskCanceledException();
            }

            Task<T> result = await Task.WhenAny(
                task,
                canceller()
            );

            return await result;
        }
    }
}
