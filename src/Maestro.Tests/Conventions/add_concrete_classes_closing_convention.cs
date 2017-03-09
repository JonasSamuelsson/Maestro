using System;
using System.Collections.Generic;
using System.Linq;
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
			var container = new Container(x => x.Scan.Types(types).For.ConcreteClassesClosing(typeof(IInterface<>), y => y.Add()));


			Should.NotThrow(() => container.GetServices<IList<IDisposable>>());
			Should.NotThrow(() => container.GetServices<IList<object>>());
			Should.Throw<ActivationException>(() => container.GetService<IList<string>>());
		}

		[Fact]
		public void should_support_instance_configuration()
		{
			var types = new[] { typeof(Class<IDisposable>) };
			var container = new Container(x => x.Scan.Types(types).For.ConcreteClassesClosing(typeof(IInterface<>), y => y.Add().Lifetime.Singleton()));

			var instances1 = container.GetServices<IInterface<IDisposable>>();
			var instances2 = container.GetServices<IInterface<IDisposable>>();

			instances1.Single().ShouldBe(instances2.Single());
		}

		private interface IInterface<T> { }
		private class Class<T> : IInterface<T> { }
		private class ClassOfObject : Class<object> { }
	}
}