using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using Voltmeter.Ports.Discovery;
using Voltmeter.UseCases;

namespace Voltmeter.UI.HostedServices
{
    public class StatusRefreshing : IHostedService
    {
        private readonly RefreshEnvironmentStatusUseCase _useCase;
        private readonly TimeSpan _healthTestInterval = TimeSpan.FromMinutes(1);
        private readonly Timer _timer;
        private readonly ILogger _logger;
        private readonly IEnvironmentDiscovery _environmentDiscovery;

        public StatusRefreshing(RefreshEnvironmentStatusUseCase useCase, ILogger logger, IEnvironmentDiscovery environmentDiscovery)
        {
            _useCase = useCase;
            _logger = logger;
            _environmentDiscovery = environmentDiscovery;
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
            foreach (var environment in _environmentDiscovery.Discover())
            {
                RefresheEnvironment(environment);
            }
        }

        private void RefresheEnvironment(Environment environment)
        {
            try
            {
                _logger.Information("Refreshing status of {Environment}", environment);

                _useCase.Refresh(environment);

                _logger.Information("Status of {Environment} successfully refreshed", environment);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not refresh status of {Environment}", environment);
            }
        }
    }
}