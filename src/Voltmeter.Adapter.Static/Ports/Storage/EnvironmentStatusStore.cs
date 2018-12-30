using System.Collections.Generic;
using System.Linq;
using Voltmeter.Ports.Storage;

namespace Voltmeter.Adapter.Static.Ports.Storage
{
    internal class EnvironmentStatusStore : IEnvironmentStatusStore
    {
        private readonly Dictionary<string, ServiceStatus[]> _environmentStatusData;

        public EnvironmentStatusStore()
        {
            _environmentStatusData = new Dictionary<string, ServiceStatus[]>();
        }

        public ServiceStatus[] GetFor(string environmentName)
        {
            if (_environmentStatusData.ContainsKey(environmentName))
            {
                return _environmentStatusData[environmentName];
            }

            return new ServiceStatus[0];
        }

        public string[] GetAvailableEnvironments()
        {
            return _environmentStatusData.Keys.ToArray();
        }

        public void Update(Environment environment, IEnumerable<ServiceStatus> results)
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