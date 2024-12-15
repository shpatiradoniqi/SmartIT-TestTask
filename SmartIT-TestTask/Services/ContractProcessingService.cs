using SmartIT_TestTask.BackgroundProccess;

namespace SmartIT_TestTask.Services
{
    public class ContractProcessingService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<ContractProcessingService> _logger;

        public ContractProcessingService(IBackgroundTaskQueue taskQueue, ILogger<ContractProcessingService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ContractProcessingService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                if (workItem != null)
                {
                    try
                    {
                        await workItem(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred executing background task.");
                    }
                }
            }

            _logger.LogInformation("ContractProcessingService is stopping.");
        }
    }
}
