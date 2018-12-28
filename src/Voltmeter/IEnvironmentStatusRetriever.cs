namespace Voltmeter
{
    public interface IEnvironmentStatusRetriever
    {
        ApplicationStatus[] GetFor(string environmentName);
    }
}