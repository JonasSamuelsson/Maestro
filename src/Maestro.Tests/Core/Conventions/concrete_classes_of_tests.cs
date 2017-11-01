﻿using Shouldly;
using System.Linq;
using Xunit;

namespace Maestro.Tests.Core.Conventions
{
	public class concrete_classes_of_tests
	{
		[Fact]
		public void should_add_concrete_classes_of_provided_type()
		{
			var types = new[] { typeof(Class) };
			var container = new Container(x => x.Scan(_ => _.Types(types).ForConcreteClassesOf<IInterface>()));

			container.GetServices<IInterface>().Single().ShouldBeOfType<Class>();
		}

		[Fact]
		public void should_add_concrete_classes_of_provided_generic_type_definition()
		{
			var types = new[] { typeof(Class<>), typeof(ClassOfInt) };
			var container = new Container(x => x.Scan(_ => _.Types(types).ForConcreteClassesOf(typeof(IInterface<>))));

			var instances = container.GetServices<IInterface<int>>().ToList();

			instances.Count.ShouldBe(2);
			instances.Count(x => x.GetType() == typeof(Class<int>)).ShouldBe(1);
			instances.Count(x => x.GetType() == typeof(ClassOfInt)).ShouldBe(1);
		}

		[Fact]
		public void should_support_instance_configuration()
		{
			var types = new[] { typeof(Class) };
			var container = new Container(x => x.Scan(_ => _.Types(types).ForConcreteClassesOf(typeof(IInterface), y => y.Add().Lifetime.Singleton())));

			var instances1 = container.GetServices<Class>().ToList();
			var instances2 = container.GetServices<Class>().ToList();

			instances1.ForEach(x => instances2.ShouldContain(x));
		}

		private interface IInterface { }
		private class Class : IInterface { }

		private interface IInterface<T> { }
		private class Class<T> : IInterface<T> { }
		private class ClassOfInt : Class<int> { }
	}
}