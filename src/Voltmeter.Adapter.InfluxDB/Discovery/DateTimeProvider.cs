using System;

namespace Voltmeter.Adapter.InfluxDB.Discovery
{
    internal class DateTimeProvider
    {
        private static Func<DateTime> _utcNowFunc = () => DateTime.UtcNow;
        
        public static DateTime UtcNow()
        {
            if (_utcNowFunc == null)
            {
                _utcNowFunc = () => DateTime.UtcNow;
            }

            return _utcNowFunc();
        }
    }
}