
using System.Collections.Concurrent;

namespace SmartIT_TestTask.BackgroundProccess
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void QueueBackgroundTask(Func<CancellationToken, Task> task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            _workItems.Enqueue(task);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, Task>?> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);
            return workItem;
        }
    }
}
