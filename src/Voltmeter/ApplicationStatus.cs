using System;

namespace Voltmeter
{
    public class ApplicationStatus
    {
        public string Name { get; set; }
        public bool IsHealthy { get; set; }
        public Uri Location { get; set; }
    }
}