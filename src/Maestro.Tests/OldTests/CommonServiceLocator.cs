﻿using System;
using CommonServiceLocator.MaestroAdapter;
using FluentAssertions;
using Microsoft.Practices.ServiceLocation;
using Xunit;

namespace Maestro.Tests
{
	public class CommonServiceLocator
	{
		[Todo]
		public void should_get_instance_via_common_service_locator_interface()
		{
			//var container = new Container(x => x.Service<IFoo>().Use<Foo>());
			//IServiceLocator locator = new MaestroServiceLocator(container);
			//var instance = locator.GetInstance<IFoo>();
			//instance.Should().NotBeNull();
		}

		[Todo]
		public void should_get_all_instances_via_common_service_locator_interface()
		{
			//var foo = new Foo();
			//var container = new Container(x => x.Service<IFoo>().Use(foo));
			//IServiceLocator locator = new MaestroServiceLocator(container);
			//var instances = locator.GetAllInstances<IFoo>();
			//instances.Should().BeEquivalentTo(new[] { foo });
		}

		private interface IFoo { }
		private class Foo : IFoo { }
	}
}