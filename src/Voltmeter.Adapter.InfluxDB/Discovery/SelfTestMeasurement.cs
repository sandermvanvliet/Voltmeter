using System;
using Vibrant.InfluxDB.Client;

namespace Voltmeter.Adapter.InfluxDB.Discovery
{
    internal class SelfTestMeasurement
    {
        [InfluxTimestamp]
        public DateTime Time { get; set; }
        [InfluxField("value")]
        public int Value { get; set; }
        [InfluxTag("jedlix.environment")]
        public string Environment { get; set; }
        [InfluxTag("jedlix.customer")]
        public string Customer { get; set; }
        [InfluxTag("jedlix.domain")]
        public string Domain { get; set; }
        [InfluxTag("jedlix.name")]
        public string Name { get; set; }
    }
}