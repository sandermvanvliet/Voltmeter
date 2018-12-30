namespace Voltmeter.Ports.Providers
{
    public interface IServiceDependenciesProvider
    {
        ServiceDependency[] ProvideFor(Service service, Environment environment);
    }
}