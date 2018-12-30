namespace Voltmeter
{
    public class DependencyStatus : Dependency
    {
        public bool  IsHealthy { get; set; }

        public static DependencyStatus HealthyFrom(Dependency dependency)
        {
            return new DependencyStatus
            {
                IsHealthy = true,
                Name = dependency.Name
            };
        }

        public static DependencyStatus UnhealthyFrom(Dependency dependency)
        {
            return new DependencyStatus
            {
                IsHealthy = false,
                Name = dependency.Name
            };
        }
    }
}