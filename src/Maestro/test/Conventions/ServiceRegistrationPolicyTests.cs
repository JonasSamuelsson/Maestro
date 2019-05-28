using Maestro.Configuration;
using Shouldly;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Maestro.Tests.Conventions
{
	public class ServiceRegistrationPolicyTests
	{
		[Fact]
		public void ShouldRegisterServiceUsingPolicy()
		{
			var container = new Container(x => x.Scan(s =>
			{
				var types = GetType().GetNestedTypes(BindingFlags.NonPublic);
				s.Types(types).UsingRegistrationPolicies();
			}));

			var service1 = container.GetService<IFoo>();
			var service2 = container.GetService<IFoo>();

			service1.ShouldBeOfType<Foo>();
			service1.ShouldBe(service2);
		}

		private interface IFoo { }

		[ServiceRegistrationPolicy(typeof(TestPolicy))]
		private class Foo : IFoo { }

		private class TestPolicy : IServiceRegistrationPolicy
		{
			public void Execute(Type type, IContainerBuilder builder)
			{
				builder.Add(type.GetInterfaces().Single()).Type(type).Singleton();
			}
		}
	}
}