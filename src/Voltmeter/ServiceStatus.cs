namespace Voltmeter
{
    public class ServiceStatus : Service
    {
        public bool IsHealthy { get; set; }
        public DependencyStatus[] Dependencies { get; set; }

        public static ServiceStatus HealthyFrom(Service service)
        {
            return new ServiceStatus
            {
                IsHealthy = true,
                Environment = service.Environment,
                Location = service.Location,
                Name = service.Name
            };
        }

        public static ServiceStatus UnhealthyFrom(Service service)
        {
            return new ServiceStatus
            {
                IsHealthy = false,
                Environment = service.Environment,
                Location = service.Location,
                Name = service.Name
            };
        }
    }
}