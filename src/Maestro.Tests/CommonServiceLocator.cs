using System;
using CommonServiceLocator.MaestroAdapter;
using FluentAssertions;
using Microsoft.Practices.ServiceLocation;
using Xunit;

namespace Maestro.Tests
{
	public class CommonServiceLocator
	{
		[Fact]
		public void should_get_instance_via_common_service_locator_interface()
		{
			throw new NotImplementedException();
			//var container = new Container(x => x.For<IFoo>().Use<Foo>());
			//IServiceLocator locator = new MaestroServiceLocator(container);
			//var instance = locator.GetInstance<IFoo>();
			//instance.Should().NotBeNull();
		}

		[Fact]
		public void should_get_all_instances_via_common_service_locator_interface()
		{
			throw new NotImplementedException();
			//var foo = new Foo();
			//var container = new Container(x => x.For<IFoo>().Use(foo));
			//IServiceLocator locator = new MaestroServiceLocator(container);
			//var instances = locator.GetAllInstances<IFoo>();
			//instances.Should().BeEquivalentTo(new[] { foo });
		}

		private interface IFoo { }
		private class Foo : IFoo { }
	}
}