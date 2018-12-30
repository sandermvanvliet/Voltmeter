using Voltmeter.Ports.Providers;

namespace Voltmeter.Adapter.Static.Ports.Providers
{
    internal class ServiceDependenciesProvider : IServiceDependenciesProvider
    {
        public DependencyStatus[] ProvideFor(Service service)
        {
            return new[]
            {
                DependencyStatus.HealthyFrom(DatabaseDependency()),
                DependencyStatus.HealthyFrom(ApiDependency()),
                DependencyStatus.UnhealthyFrom(MessageBusDependency()),
            };
        }

        private static Dependency DatabaseDependency()
        {
            return new Dependency
            {
                Name = "database"
            };
        }

        private static Dependency MessageBusDependency()
        {
            return new Dependency
            {
                Name = "messagebus"
            };
        }

        private static Dependency ApiDependency()
        {
            return new Dependency
            {
                Name = "session"
            };
        }
    }
}