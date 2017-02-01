using System;
using System.Linq;
using Maestro.Lifetimes;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Diagnostics
{
	public class get_configuration_tests
	{
		[Fact]
		public void should_list_services_in_alphabetical_order()
		{
			var container = new Container(x =>
			{
				x.For<B>().Use.Self();
				x.For<A>().Use.Self();
				x.For<B>("x").Use.Self();
				x.For<A>("x").Use.Self();
				x.For<B>().Add.Self();
				x.For<A>().Add.Self();
			});

			var configuration = container.GetConfiguration();

			configuration.Services.ElementAt(0).ServiceType.ShouldBe(typeof(A));
			configuration.Services.ElementAt(0).Name.ShouldBe(string.Empty);
			configuration.Services.ElementAt(1).ServiceType.ShouldBe(typeof(A));
			configuration.Services.ElementAt(1).Name.ShouldBe("x");
			configuration.Services.ElementAt(2).ServiceType.ShouldBe(typeof(A));
			configuration.Services.ElementAt(2).Name.ShouldBe(null);
			configuration.Services.ElementAt(3).ServiceType.ShouldBe(typeof(B));
			configuration.Services.ElementAt(3).Name.ShouldBe(string.Empty);
			configuration.Services.ElementAt(4).ServiceType.ShouldBe(typeof(B));
			configuration.Services.ElementAt(4).Name.ShouldBe("x");
			configuration.Services.ElementAt(5).ServiceType.ShouldBe(typeof(B));
			configuration.Services.ElementAt(5).Name.ShouldBe(null);
		}

		[Fact]
		public void should_include_lifetime()
		{
			var container = new Container(x =>
			{
				x.For<A>().Add.Self().Lifetime.Transient();
				x.For<A>().Add.Self().Lifetime.Context();
				x.For<A>().Add.Self().Lifetime.Singleton();
				x.For<A>().Add.Self().Lifetime.Use<MyCustomLifetime>();
			});

			var configuration = container.GetConfiguration();

			configuration.Services.ElementAt(0).Lifetime.ShouldBe("Transient");
			configuration.Services.ElementAt(1).Lifetime.ShouldBe("Context singleton");
			configuration.Services.ElementAt(2).Lifetime.ShouldBe("Singleton");
			configuration.Services.ElementAt(3).Lifetime.ShouldBe("Maestro.Tests.Diagnostics.get_configuration_tests+MyCustomLifetime");
		}

		[Fact]
		public void should_include_provider_and_instance_type()
		{
			var container = new Container(x =>
			{
				x.For<IObject>().Add.Instance(new A());
				x.For<IObject>().Add.Factory(() => new A());
				x.For<IObject>().Add.Type<B>();
			});

			var configuration = container.GetConfiguration();

			configuration.Services.ElementAt(0).Provider.ShouldBe("Instance");
			configuration.Services.ElementAt(0).InstanceType.ShouldBe(typeof(A));
			configuration.Services.ElementAt(1).Provider.ShouldBe("Factory");
			configuration.Services.ElementAt(1).InstanceType.ShouldBe(null);
			configuration.Services.ElementAt(2).Provider.ShouldBe("Type");
			configuration.Services.ElementAt(2).InstanceType.ShouldBe(typeof(B));
		}

		interface IObject { }
		class A : IObject { }
		class B : IObject { }

		public class MyCustomLifetime : ILifetime
		{
			public object Execute(IContext context, Func<IContext, object> factory)
			{
				throw new NotImplementedException();
			}

			public ILifetime MakeGeneric(Type[] genericArguments)
			{
				throw new NotImplementedException();
			}
		}
	}
}