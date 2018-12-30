namespace Voltmeter.Ports.Providers
{
    public interface IServiceDependenciesProvider
    {
        DependencyStatus[] ProvideFor(Service service);
    }
}