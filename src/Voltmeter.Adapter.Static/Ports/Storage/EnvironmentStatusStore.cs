using System.Collections.Generic;
using System.Linq;
using Voltmeter.Ports.Storage;

namespace Voltmeter.Adapter.Static.Ports.Storage
{
    public class EnvironmentStatusStore : IEnvironmentStatusStore
    {
        private readonly Dictionary<string, ApplicationStatus[]> _environmentStatusData;

        public EnvironmentStatusStore()
        {
            _environmentStatusData = new Dictionary<string, ApplicationStatus[]>();
        }

        public ApplicationStatus[] GetFor(string environmentName)
        {
            if (_environmentStatusData.ContainsKey(environmentName))
            {
                return _environmentStatusData[environmentName];
            }

            return new ApplicationStatus[0];
        }

        public string[] GetAvailableEnvironments()
        {
            return _environmentStatusData.Keys.ToArray();
        }

        public void Update(Environment environment, IEnumerable<ApplicationStatus> results)
        {
            if (_environmentStatusData.ContainsKey(environment.Name))
            {
                _environmentStatusData[environment.Name] = results.ToArray();
            }
            else
            {
                _environmentStatusData.Add(environment.Name, results.ToArray());
            }
        }
    }
}