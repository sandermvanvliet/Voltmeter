using System;
using Voltmeter.Ports.Providers;

namespace Voltmeter.UseCases
{
    public class RefreshServiceStatusUseCase
    {
        private readonly IServiceStatusProvider _statusProvider;
        private readonly IServiceDependenciesProvider _dependenciesProvider;

        public RefreshServiceStatusUseCase(IServiceStatusProvider statusProvider, IServiceDependenciesProvider dependenciesProvider)
        {
            _statusProvider = statusProvider;
            _dependenciesProvider = dependenciesProvider;
        }

        public ServiceStatus Refresh(Service service)
        {
            return null;
        }
    }
}