using System;
using Voltmeter.Ports.Providers;

namespace Voltmeter.UseCases
{
    public class RefreshServiceStatusUseCase
    {
        private readonly IServiceStatusProvider _statusProvider;
        private readonly IServiceDependenciesProvider _dependenciesProvider;

        public RefreshServiceStatusUseCase(
            IServiceStatusProvider statusProvider, 
            IServiceDependenciesProvider dependenciesProvider)
        {
            _statusProvider = statusProvider;
            _dependenciesProvider = dependenciesProvider;
        }

        public ServiceStatus Refresh(Service service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var status = _statusProvider.ProvideFor(service);

            if (status != null)
            {
                try
                {
                    var dependencies = _dependenciesProvider.ProvideFor(service);

                    status.Dependencies = dependencies;
                }
                catch
                {
                    // Nop
                }
            }

            return status;
        }
    }
}