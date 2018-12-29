namespace Voltmeter
{
    public class EnvironmentDiscovery : IEnvironmentDiscovery
    {
        public string[] Discover()
        {
            return new[]
            {
                "production",
                "staging",
                "acceptance",
                "test"
            };
        }
    }
}