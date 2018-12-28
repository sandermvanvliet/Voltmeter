using System.Collections;

namespace Voltmeter.UI.Models
{
    public class EnvironmentStatusModel
    {
        public ApplicationModel[] Applications { get; set; }
        public string Environment { get; set; }
        public DependencyModel[] Edges { get; set; }
    }
}