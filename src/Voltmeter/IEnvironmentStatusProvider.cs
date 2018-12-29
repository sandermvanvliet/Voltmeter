namespace Voltmeter
{
    public interface IEnvironmentStatusProvider
    {
        ApplicationStatus[] ProvideFor(string environmentName);
    }
}