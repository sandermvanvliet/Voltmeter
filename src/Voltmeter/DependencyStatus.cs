namespace Voltmeter
{
    public class DependencyStatus : Dependency
    {
        public bool  IsHealthy { get; set; }

        public static DependencyStatus HealthyFrom(Dependency dependency)
        {
            return new DependencyStatus
            {
                IsHealthy = true
            };
        }

        public static DependencyStatus UnhealthyFrom(Dependency dependency)
        {
            return new DependencyStatus
            {
                IsHealthy = false
            };
        }
    }
}