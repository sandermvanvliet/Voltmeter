namespace Voltmeter.UI.Models
{
    public class EnvironmentStatusModel
    {
        public ServiceModel[] Services { get; set; }
        public string Environment { get; set; }
        public DependencyModel[] Edges { get; set; }
        public string[] AvailableEnvironments { get; set; }
    }
}