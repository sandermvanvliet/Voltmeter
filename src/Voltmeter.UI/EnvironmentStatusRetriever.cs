namespace Voltmeter.UI
{
    public class EnvironmentStatusRetriever : IEnvironmentStatusRetriever
    {
        public ApplicationStatus[] GetFor(string environmentName)
        {
            return new ApplicationStatus[0];
        }

        public string[] GetAvailableEnvironments()
        {
            throw new System.NotImplementedException();
        }
    }
}