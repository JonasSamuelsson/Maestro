using Maestro.Configuration;
using Shouldly;
using System.Linq;
using Xunit;

namespace Maestro.Tests.Conventions
{
	public class concrete_classes_of_tests
	{
		[Fact]
		public void should_add_concrete_classes_of_provided_type()
		{
			var types = new[] { typeof(Class) };
			var container = new Container(x => x.Scan(_ => _.Types(types).RegisterConcreteClassesOf<IInterface>()));

			container.GetServices<IInterface>().Single().ShouldBeOfType<Class>();
		}

		[Fact]
		public void should_add_concrete_classes_of_provided_generic_type_definition()
		{
			var types = new[] { typeof(Class<>), typeof(ClassOfInt) };
			var container = new Container(x => x.Scan(_ => _.Types(types).RegisterConcreteClassesOf(typeof(IInterface<>))));

			var instances = container.GetServices<IInterface<int>>().ToList();

			instances.Count.ShouldBe(2);
			instances.Count(x => x.GetType() == typeof(Class<int>)).ShouldBe(1);
			instances.Count(x => x.GetType() == typeof(ClassOfInt)).ShouldBe(1);
		}

		[Fact]
		public void should_support_instance_configuration()
		{
			var types = new[] { typeof(Class) };
			var container = new Container(x => x.Scan(_ => _.Types(types).RegisterConcreteClassesOf(typeof(IInterface), y => y.Add().Singleton())));

			var instance1 = container.GetService<IInterface>();
			var instance2 = container.GetService<IInterface>();

			instance1.ShouldBe(instance2);
		}

		private interface IInterface { }
		private class Class : IInterface { }

		private interface IInterface<T> { }
		private class Class<T> : IInterface<T> { }
		private class ClassOfInt : Class<int> { }
	}
}