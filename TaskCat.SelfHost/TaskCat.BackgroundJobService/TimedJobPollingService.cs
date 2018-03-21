using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TaskCat.PartnerServices.Infini;

namespace TaskCat.BackgroundJobService
{
    public class TimedJobPollingService: IHostedService, IDisposable
    {
        private ILogger _logger;
        private Timer _timer;
        private HttpClient _httpClient;
        private OrderService _orderService;
        private string _infiniToken;

        public TimedJobPollingService(ILogger<TimedJobPollingService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _orderService = new OrderService(_httpClient);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            try
            {
                if (string.IsNullOrWhiteSpace(this._infiniToken))
                {
                    this._infiniToken = await this._orderService.Login();
                }

            }
            catch (Exception)
            {
                // relogin here
                throw;
            }
           
        }

        public async Task<bool> ValidToken()
        {
            OrderService orderService = new OrderService(_httpClient);
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
