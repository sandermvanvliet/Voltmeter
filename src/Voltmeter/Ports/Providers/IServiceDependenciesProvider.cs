namespace Voltmeter.Ports.Providers
{
    public interface IServiceDependenciesProvider
    {
        Dependency[] ProvideFor(Service service);
    }
}