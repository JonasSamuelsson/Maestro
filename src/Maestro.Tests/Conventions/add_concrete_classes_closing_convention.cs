using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Conventions
{
	public class add_concrete_classes_closing_convention
	{
		[Fact]
		public void should_register_type_closing_provided_generic_type_definition()
		{
			var types = new[] { typeof(Class<IDisposable>), typeof(ClassOfObject) };
			var container = new Container(x => x.Scan.Types(types).AddConcreteClassesClosing(typeof(IInterface<>)));

			container.Invoking(x => x.GetServices<IList<IDisposable>>()).ShouldNotThrow("IInterface<IDisposable>");
			container.Invoking(x => x.GetServices<IList<object>>()).ShouldNotThrow("IInterface<object>");
			container.Invoking(x => x.GetService<IList<string>>()).ShouldThrow<ActivationException>();
		}

		[Fact]
		public void should_support_instance_configuration()
		{
			var types = new[] { typeof(Class<IDisposable>) };
			var container = new Container(x => x.Scan.Types(types).AddConcreteClassesClosing(typeof(IInterface<>), y => y.Lifetime.Singleton()));

			var instances1 = container.GetServices<IInterface<IDisposable>>();
			var instances2 = container.GetServices<IInterface<IDisposable>>();

			instances1.Single().ShouldBe(instances2.Single());
		}

		private interface IInterface<T> { }
		private class Class<T> : IInterface<T> { }
		private class ClassOfObject : Class<object> { }
	}
}