using System.Linq;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.Conventions
{
	public class ConcreteClassesOfConvention_tests
	{
		[Fact]
		public void should_register_concrete_classes_of_provided_type()
		{
			var types = new[] { typeof(Class) };
			var container = new Container(x => x.Scan(_ => _.Types(types).ForConcreteClassesOf<IInterface>(y => y.Use())));

			container.GetService<IInterface>().ShouldBeOfType<Class>();
		}

		[Fact]
		public void should_register_concrete_classes_of_provided_generic_type_definition()
		{
			var types = new[] { typeof(Class<>), typeof(ClassOfInt) };
			var container = new Container(x => x.Scan(_ => _.Types(types).ForConcreteClassesOf(typeof(IInterface<>), y => y.Add())));

			var instances = container.GetServices<IInterface<int>>().ToList();

			instances.Count.ShouldBe(2);
			instances.Count(x => x.GetType() == typeof(Class<int>)).ShouldBe(1);
			instances.Count(x => x.GetType() == typeof(ClassOfInt)).ShouldBe(1);
		}

		[Fact]
		public void should_support_instance_configuration()
		{
			var types = new[] { typeof(Class) };

			var container1 = new Container(x => x.Scan(_ => _.Types(types).ForConcreteClassesOf(typeof(IInterface), y => y.Add().Lifetime.Singleton())));
			var instance1 = container1.GetServices<IInterface>().Single();
			var instance2 = container1.GetServices<IInterface>().Single();
			instance1.ShouldBe(instance2);

			var container2 = new Container(x => x.Scan(_ => _.Types(types).ForConcreteClassesOf<IInterface>(y => y.Add().Lifetime.Singleton())));
			var instance3 = container2.GetServices<IInterface>().Single();
			var instance4 = container2.GetServices<IInterface>().Single();
			instance3.ShouldBe(instance4);
		}

		private interface IInterface { }
		private class Class : IInterface { }

		private interface IInterface<T> { }
		private class Class<T> : IInterface<T> { }
		private class ClassOfInt : Class<int> { }
	}
}