using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Maestro.Tests
{
	public class type_instance_with_Enumerable_dependency
	{
		[Fact]
		public void should_get_instance_with_configured_dependency()
		{
			var container = new Container(x =>
			{
				x.Default<ClassWithEnumerableDependency>().Type<ClassWithEnumerableDependency>();
				x.Default<IEnumerable<object>>().Type<List<object>>();
			});

			var o = container.Get<ClassWithEnumerableDependency>();

			o.Dependencies.Should().BeOfType<List<object>>();
			o.Dependencies.Should().BeEmpty();
		}

		[Fact]
		public void should_get_instance_with_configured_dependencies()
		{
			var container = new Container(x =>
			{
				x.Default<ClassWithEnumerableDependency>().Type<ClassWithEnumerableDependency>();
				x.Add<object>().Type<object>();
				x.Add<object>().Type<Exception>();
				x.Add<object>().Type<EventArgs>();
			});

			var o = container.Get<ClassWithEnumerableDependency>();

			o.Dependencies.Should().HaveCount(3);
			o.Dependencies.Should().Contain(x => x.GetType() == typeof(object));
			o.Dependencies.Should().Contain(x => x.GetType() == typeof(Exception));
			o.Dependencies.Should().Contain(x => x.GetType() == typeof(EventArgs));
		}

		[Fact]
		public void should_get_instance_with_unregistered_dependencies()
		{
			var container = new Container(x => x.Default<ClassWithEnumerableDependency>().Type<ClassWithEnumerableDependency>());

			var o = container.Get<ClassWithEnumerableDependency>();

			o.Dependencies.Should().BeEmpty();
		}

		private class ClassWithEnumerableDependency
		{
			public ClassWithEnumerableDependency(IEnumerable<object> dependencies)
			{
				Dependencies = dependencies;
			}

			public IEnumerable<object> Dependencies { get; private set; }
		}
	}
}