using System;

namespace Voltmeter.Exceptions
{
    public class ServiceNotFoundException : Exception
    {
        public ServiceNotFoundException(Service service) 
            : base((string) "Service was not found")
        {
            Data.Add("service", service.Name);
        }
    }
}