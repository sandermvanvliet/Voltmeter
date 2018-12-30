namespace Voltmeter.Ports.Providers
{
    public interface IServiceStatusProvider
    {
        ServiceStatus ProvideFor(Service service, Environment environment);
    }
}
