using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class named_type_instance_with_configured_dependencies
	{
		[Fact]
		public void should_get_instance_with_dependency_with_same_name()
		{
			var name = "foo";
			var container = new Container(x =>
			{
				x.For<ClassWithDependency>(name).Use.Type<ClassWithDependency>();
				x.For<object>(name).Use.Type<object>();
			});

			var o = container.GetService<ClassWithDependency>(name);

			o.Dependency.ShouldBeOfType<object>();
		}

		[Fact]
		public void should_use_default_instance_for_dependency_if_one_with_same_name_not_present()
		{
			var name = "bar";
			var container = new Container(x =>
			{
				x.For<ClassWithDependency>(name).Use.Type<ClassWithDependency>();
				x.For<object>().Use.Type<EventArgs>();
			});

			var o = container.GetService<ClassWithDependency>(name);

			o.Dependency.ShouldBeOfType<EventArgs>();
		}

		private class ClassWithDependency
		{
			public ClassWithDependency(object dependency)
			{
				Dependency = dependency;
			}

			public object Dependency { get; private set; }
		}
	}
}