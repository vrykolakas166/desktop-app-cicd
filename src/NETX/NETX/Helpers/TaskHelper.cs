namespace NETX.Helpers
{
    public static class TaskHelper
    {
        public static async void RunAfter(TimeSpan delay, Action action)
        {
            await Task.Delay(delay);
            action.Invoke();
        }

        public static async Task RunAfterAsync(TimeSpan delay, Action action)
        {
            await Task.Delay(delay);
            action.Invoke();
        }
    }
}
