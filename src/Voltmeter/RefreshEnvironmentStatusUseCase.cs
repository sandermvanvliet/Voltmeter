using System;
using System.Linq;
using Serilog;

namespace Voltmeter
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

        public void Refresh(string environment)
        {
            if (string.IsNullOrWhiteSpace(environment))
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