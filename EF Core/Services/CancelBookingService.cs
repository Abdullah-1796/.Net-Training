
using EF_Core.DTOs;
using EF_Core.Services.Interfaces;

namespace EF_Core.Services
{
    public class CancelBookingService(ILogger<CancelBookingService> logger, IServiceProvider serviceProvider) : IHostedService, IDisposable
    {
        private System.Timers.Timer? _timer;
        private readonly ILogger<CancelBookingService> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        private async Task CancelBooking()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                IBookingService bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>()!;
                bool isCanceled = await bookingService.CancelPostponedBookingsAsync();
                if (isCanceled)
                {
                    _logger.LogInformation("Postponed bookings have been canceled");
                }
                else
                {
                    _logger.LogInformation("Error while cancelling postponed bookings");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception while cancelation of postponed bookings: {ex}");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new System.Timers.Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
            _timer.Elapsed += async (sender, args) => await CancelBooking();
            _timer.AutoReset = true;
            _timer.Enabled = true;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
