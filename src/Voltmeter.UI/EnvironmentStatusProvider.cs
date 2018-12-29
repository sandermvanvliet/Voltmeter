namespace Voltmeter.UI
{
    internal class EnvironmentStatusProvider: IEnvironmentStatusProvider
    {
        public ApplicationStatus[] ProvideFor(string environmentName)
        {
            return new ApplicationStatus[0];
        }
    }
}