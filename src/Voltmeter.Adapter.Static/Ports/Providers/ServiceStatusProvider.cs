﻿using Voltmeter.Ports.Providers;

namespace Voltmeter.Adapter.Static.Ports.Providers
{
    internal class ServiceStatusProvider : IServiceStatusProvider
    {
        public ServiceStatus ProvideFor(Service service)
        {
            return ServiceStatus.HealthyFrom(service);
        }
    }
}