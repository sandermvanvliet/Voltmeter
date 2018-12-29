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
            return new string[0];
        }

        public void Update(string environment, ApplicationStatus[] results)
        {
            throw new System.NotImplementedException();
        }
    }
}