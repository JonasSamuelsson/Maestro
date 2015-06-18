using System;
using System.Collections.Generic;
using FluentAssertions;
using Maestro.Interceptors;
using Maestro.Lifetimes;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class open_generic_type
	{
		[Fact]
		public void should_get_generic_type()
		{
			var container = new Container(x => x.For(typeof(Instance<>)).Use(typeof(Instance<>)));

			var instance = container.Get<Instance<int>>();

			instance.ShouldBeOfType<Instance<int>>();
		}

		[Fact]
		public void should_execute_configured_interceptors()
		{
			var container = new Container(x => x.For(typeof(Instance<>)).Use(typeof(Instance<>)).SetProperty("Value", "foobar"));

			var instance = container.Get<Instance<string>>();

			instance.Value.ShouldBe("foobar");
		}

		[Fact]
		public void child_container_config_should_be_considered_over_parent_container()
		{
			var parent = new Container(x => x.For<Instance<string>>().Use(() => new Instance<string> { Value = "root" }));
			var child = parent.GetChildContainer(x => x.For(typeof(Instance<>)).Use(typeof(Instance<>)));

			var instance = child.Get<Instance<string>>();

			instance.Value.ShouldBe(null);
		}

		[Todo]
		public void configured_interceptors_and_lifetimes_should_be_cloned_and_clone_should_be_executed()
		{
			//var lifetime = new Lifetime();
			//var interceptor = new Interceptor();

			//new Container(x => x.For(typeof(IList<>)).Use(typeof(List<>))
			//	.Lifetime.Use(lifetime)
			//	.Intercept(interceptor))
			//	.Get<IList<int>>();

			//lifetime.IsCloned.Should().BeTrue();
			//lifetime.Executed.Should().BeFalse();
			//lifetime.Clone.IsCloned.Should().BeFalse();
			//lifetime.Clone.Executed.Should().BeTrue();

			//interceptor.IsCloned.Should().BeTrue();
			//interceptor.Executed.Should().BeFalse();
			//interceptor.Clone.IsCloned.Should().BeFalse();
			//interceptor.Clone.Executed.Should().BeTrue();
		}

		class Instance<T>
		{
			public T Value { get; set; }
		}
	}
}