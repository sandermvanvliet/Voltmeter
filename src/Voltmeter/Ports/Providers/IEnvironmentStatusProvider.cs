namespace Voltmeter.Ports.Providers
{
    public interface IEnvironmentStatusProvider
    {
        ApplicationStatus[] ProvideFor(Environment environment);
    }
}