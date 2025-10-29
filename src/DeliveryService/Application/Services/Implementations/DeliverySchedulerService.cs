using DeliveryService.Application.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryService.Application.Services.Implementations
{
    public class DeliverySchedulerService : IHostedService, IDisposable
    {
        private Timer _timer = null!;
        private readonly IServiceProvider _serviceProvider;

        public DeliverySchedulerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RunScheduledTasks, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void RunScheduledTasks(object? state)
        {
            using var scope = _serviceProvider.CreateScope();
            var deliveryService = scope.ServiceProvider.GetRequiredService<IDeliveryService>();

            var now = DateTime.Now;

            try
            {
                if (now.Hour == 8 && now.Minute == 0)
                    await deliveryService.ProcessPendingDeliveriesAsync();

                if (now.Hour == 12 && now.Minute == 0)
                    await deliveryService.ProcessDeliveriesToProcessAsync();

                if (now.Hour == 17 && now.Minute == 0)
                    await deliveryService.ProcessOnRouteDeliveriesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeliveryScheduler error: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
