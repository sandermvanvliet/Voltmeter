namespace Voltmeter
{
    public class ServiceStatus : Service
    {
        public ServiceHealth Health { get; set; } = ServiceHealth.Unknown;
        public DependencyStatus[] Dependencies { get; set; }

        public static ServiceStatus HealthyFrom(Service service)
        {
            return new ServiceStatus
            {
                Health = ServiceHealth.Healthy,
                Environment = service.Environment,
                Location = service.Location,
                Name = service.Name
            };
        }

        public static ServiceStatus DegradedFrom(Service service)
        {
            return new ServiceStatus
            {
                Health = ServiceHealth.Degraded,
                Environment = service.Environment,
                Location = service.Location,
                Name = service.Name
            };
        }

        public static ServiceStatus UnhealthyFrom(Service service)
        {
            return new ServiceStatus
            {
                Health = ServiceHealth.Unhealthy,
                Environment = service.Environment,
                Location = service.Location,
                Name = service.Name
            };
        }

        public static ServiceStatus UnknownFrom(Service service)
        {
            return new ServiceStatus
            {
                Health = ServiceHealth.Unknown,
                Environment = service.Environment,
                Location = service.Location,
                Name = service.Name
            };
        }
    }
}