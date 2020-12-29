using System;
using Vibrant.InfluxDB.Client;

namespace Voltmeter.Adapter.InfluxDB.Providers
{
    internal class HealthTestMeasurement
    {
        [InfluxTimestamp]
        public DateTime Time { get; set; }
        [InfluxField("value")]
        public int Value { get; set; }
        [InfluxTag("test")] 
        public string Test { get; set; }
    }
}