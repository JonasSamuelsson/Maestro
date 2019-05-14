using Maestro.Configuration;
using Shouldly;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Maestro.Tests.Conventions
{
	public class RegistrationPolicyTests
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

		[ServiceRegistrationPolicy(typeof(CustomPolicy))]
		private class Foo : IFoo { }

		private class CustomPolicy : IServiceRegistrationPolicy
		{
			public void Register(Type type, IContainerBuilder builder)
			{
				builder.Add(type.GetInterfaces().Single()).Type(type).Singleton();
			}
		}
	}

	public class ConfigurationPolicyTests
	{
		[Fact]
		public void ShouldConfigureServicesUsingPolicy()
		{
			var container = new Container(x => x.Scan(s =>
			{
				var types = GetType().GetNestedTypes(BindingFlags.NonPublic);
				s.Types(types).UsingRegistrationPolicies();
			}));
		}

		private class Service
		{
			public string String1 { get; set; }
			public string String2 { get; set; }
		}

		private class ServiceA : Service { }

		[ServiceConfigurationPolicy(typeof(CustomPolicy))]
		private class ServiceB : Service { }

		private class CustomPolicy { }
	}
}