using System.Collections.Generic;

namespace Voltmeter.Adapter.InfluxDB
{
    internal class InfluxDbCache
    {
        private readonly Dictionary<string, object> _store = new Dictionary<string, object>();

        public void Store(string key, object value)
        {
            if (_store.ContainsKey(key))
            {
                _store[key] = value;
            }
            else
            {
                _store.Add(key, value);
            }
        }

        public TValue Get<TValue>(string key)
        {
            if (_store.ContainsKey(key))
            {
                return (TValue) _store[key];
            }

            return default(TValue);
        }
    }
}