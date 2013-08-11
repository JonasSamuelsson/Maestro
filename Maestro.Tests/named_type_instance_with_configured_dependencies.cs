using FluentAssertions;
using System;
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
				x.Add<ClassWithDependency>(name).Use<ClassWithDependency>();
				x.Add<object>(name).Use<Exception>();
			});

			var o = container.Get<ClassWithDependency>(name);

			o.Dependency.Should().BeOfType<Exception>();
		}

		[Fact]
		public void should_use_default_instance_for_dependency_if_one_with_same_name_not_present()
		{
			var name = "bar";
			var container = new Container(x =>
			{
				x.Add<ClassWithDependency>(name).Use<ClassWithDependency>();
				x.For<object>().Use<EventArgs>();
			});

			var o = container.Get<ClassWithDependency>(name);

			o.Dependency.Should().BeOfType<EventArgs>();
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