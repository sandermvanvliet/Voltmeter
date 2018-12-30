using System;
using System.Linq;
using Serilog;
using Voltmeter.Ports.Providers;
using Voltmeter.Ports.Storage;

namespace Voltmeter.UseCases
{
    public class RefreshEnvironmentStatusUseCase
    {
        private readonly IEnvironmentStatusProvider _provider;
        private readonly IEnvironmentStatusStore _store;
        private readonly ILogger _logger;

        public RefreshEnvironmentStatusUseCase(
            IEnvironmentStatusProvider provider,
            IEnvironmentStatusStore store,
            ILogger logger)
        {
            _provider = provider;
            _store = store;
            _logger = logger;
        }

        public void Refresh(Environment environment)
        {
            if (environment == null || string.IsNullOrWhiteSpace(environment.Name))
            {
                throw new ArgumentNullException(nameof(environment));
            }

            try
            {
                var results = _provider.ProvideFor(environment);

                if (results.Any())
                {
                    _store.Update(environment, results);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not get status of {Environment}", environment);
                // Swallow
            }
        }
    }
}