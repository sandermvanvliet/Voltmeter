using System;
using System.Linq.Expressions;
using Moq;

namespace Voltmeter.Tests.Unit
{
    public static class It
    {
        public static Service IsService(Service service)
        {
            Expression<Func<Service, bool>> match = s => s.Name == service.Name;

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