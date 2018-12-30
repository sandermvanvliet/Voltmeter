using System;
using System.Linq.Expressions;
using Moq;

namespace Voltmeter.Tests.Unit
{
    public static class It
    {
        public static Service IsService(Service service)
        {
            return IsService(service.Name);
        }
        public static Service IsService(string serviceName)
        {
            Expression<Func<Service, bool>> match = s => s.Name == serviceName;

            return Match.Create(
                value => match.Compile().Invoke(value),
                () => Moq.It.Is(match));
        }

        public static Environment IsEnvironment(string environment)
        {
            Expression<Func<Environment, bool>> match = s => s.Name == environment;

            return Match.Create(
                value => match.Compile().Invoke(value),
                () => Moq.It.Is(match));
        }

        public static Environment IsEnvironment(Environment environment)
        {
            return IsEnvironment(environment.Name);
        }
    }
}