using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TaskCat.PartnerModels;
using TaskCat.PartnerServices.Infini;

namespace TaskCat.BackgroundJobService
{
    public class TimedJobPollingService: IHostedService, IDisposable
    {
        private ILogger _logger;
        private Timer _timer;
        private HttpClient httpClient;
        private object httpClient1;

        public TimedJobPollingService(ILogger<TimedJobPollingService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");
            ValidToken();
        }

        public async Task<bool> ValidToken()
        {
            OrderService orderService = new OrderService(httpClient, httpClient1);
            string logInToken = await orderService.Login();
            if (logInToken != null)
                return true;
            else
                return false;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
