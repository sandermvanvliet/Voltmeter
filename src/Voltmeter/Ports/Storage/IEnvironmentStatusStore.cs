namespace Voltmeter.Ports.Storage
{
    public interface IEnvironmentStatusStore
    {
        ApplicationStatus[] GetFor(string environmentName);
        string[] GetAvailableEnvironments();
        void Update(string environment, ApplicationStatus[] results);
    }
}