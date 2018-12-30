namespace Voltmeter.Ports.Providers
{
    public interface IEnvironmentStatusProvider
    {
        ApplicationStatus[] ProvideFor(string environmentName);
    }
}