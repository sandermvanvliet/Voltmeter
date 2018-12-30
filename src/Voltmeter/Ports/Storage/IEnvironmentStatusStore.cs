using System.Collections.Generic;

namespace Voltmeter.Ports.Storage
{
    public interface IEnvironmentStatusStore
    {
        ServiceStatus[] GetFor(string environmentName);
        string[] GetAvailableEnvironments();
        void Update(Environment environment, IEnumerable<ServiceStatus> results);
    }
}