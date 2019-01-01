namespace Voltmeter.Adapter.SimpleServiceStatus.Ports.Discovery
{
    internal class Settings
    {
        public string[] Environments { get; set; }
        public ServiceSetting[] Services { get; set; }
    }

    internal class ServiceSetting
    {
        public string Name { get; set; }
        public string LocationPattern { get; set; }
    }
}