
namespace DeliveryService.Application.Services.Implementations
{
    public class DeliverySchedulerService : IHostedService, IDisposable
    {
        private Timer _timer = null!;
        private readonly DeliveryService _deliveryService;

        public DeliverySchedulerService(DeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RunScheduledTasks, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void RunScheduledTasks(object? state)
        {
            var now = DateTime.Now;

            if (now.Hour == 8 && now.Minute == 0)
                await _deliveryService.ProcessPendingDeliveriesAsync();

            if (now.Hour == 12 && now.Minute == 0)
                await _deliveryService.ProcessDeliveriesToProcessAsync();

            if (now.Hour == 17 && now.Minute == 0)
                await _deliveryService.ProcessOnRouteDeliveriesAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}