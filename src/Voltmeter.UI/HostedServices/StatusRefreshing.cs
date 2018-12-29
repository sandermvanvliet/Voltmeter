using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Voltmeter.UI.HostedServices
{
    public class StatusRefreshing : IHostedService
    {
        private readonly RefreshEnvironmentStatusUseCase _useCase;
        private readonly TimeSpan _healthTestInterval = TimeSpan.FromMinutes(1);
        private readonly Timer _timer;
        private readonly ILogger _logger;

        public StatusRefreshing(RefreshEnvironmentStatusUseCase useCase, ILogger logger)
        {
            _useCase = useCase;
            _logger = logger;
            _timer = new Timer(state => Refresh(), null, Timeout.InfiniteTimeSpan, TimeSpan.Zero);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer.Change(TimeSpan.Zero, _healthTestInterval);

            _logger.Information("Status refresh timer started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.InfiniteTimeSpan, TimeSpan.Zero);
            
            _logger.Information("Status refresh timer stopped");

            return Task.CompletedTask;
        }

        private void Refresh()
        {
            var environmentName = "production";

            try
            {
                _logger.Information("Refreshing status of {Environment}", environmentName);

                _useCase.Refresh(environmentName);

                _logger.Information("Status of {Environment} successfully refreshed", environmentName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not refresh status of {Environment}", environmentName);
            }
        }
    }
}