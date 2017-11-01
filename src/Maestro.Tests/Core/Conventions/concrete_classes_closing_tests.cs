﻿using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Maestro.Tests.Core.Conventions
{
	public class concrete_classes_closing_tests
	{
		[Fact]
		public void should_add_types_closing_provided_generic_type_definition()
		{
			var types = new[] { typeof(Class<object>), typeof(ClassOfObject) };

			var container = new Container(x => x.Scan(_ => _.Types(types).ForConcreteClassesClosing(typeof(IInterface<>))));
			container.GetServices<IInterface<object>>().Count().ShouldBe(2);
		}

		[Fact]
		public void should_support_instance_configuration()
		{
			var types = new[] { typeof(Class<IDisposable>) };
			var container = new Container(x => x.Scan(_ => _.Types(types).ForConcreteClassesClosing(typeof(IInterface<>), y => y.Use().Lifetime.Singleton())));

			var instance1 = container.GetService<IInterface<IDisposable>>();
			var instance2 = container.GetService<IInterface<IDisposable>>();

			instance1.ShouldBe(instance2);
		}

		private interface IInterface<T> { }
		private class Class<T> : IInterface<T> { }
		private class ClassOfObject : Class<object> { }
	}
}