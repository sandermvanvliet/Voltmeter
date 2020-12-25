namespace Voltmeter
{
    public class DependencyStatus : Dependency
    {
        public ServiceHealth Health { get; set; } = ServiceHealth.Unknown;

        public static DependencyStatus HealthyFrom(Dependency dependency)
        {
            return new DependencyStatus
            {
                Health = ServiceHealth.Healthy,
                Name = dependency.Name
            };
        }

        public static DependencyStatus DegradedFrom(Dependency dependency)
        {
            return new DependencyStatus
            {
                Health = ServiceHealth.Degraded,
                Name = dependency.Name
            };
        }

        public static DependencyStatus UnhealthyFrom(Dependency dependency)
        {
            return new DependencyStatus
            {
                Health = ServiceHealth.Unhealthy,
                Name = dependency.Name
            };
        }
    }
}