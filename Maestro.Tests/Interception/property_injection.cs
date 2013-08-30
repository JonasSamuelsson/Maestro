using System;
using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Interception
{
	public class property_injection
	{
		[Fact]
		public void set_property_with_provided_value()
		{
			var dependency = new object();
			var container = new Container(x => x.For<Foobar>().Use<Foobar>().OnCreate.SetProperty(y => y.ResolvableDependency, dependency));

			var instance = container.Get<Foobar>();

			instance.ResolvableDependency.Should().Be(dependency);
		}

		[Fact]
		public void set_property_with_value_from_provided_func()
		{
			var dependency = new object();
			var container = new Container(x => x.For<Foobar>().Use<Foobar>().OnCreate.SetProperty(y => y.ResolvableDependency, () => dependency));

			var instance = container.Get<Foobar>();

			instance.ResolvableDependency.Should().Be(dependency);
		}

		[Fact]
		public void set_property_with_resolvable_type_should_work()
		{
			var container = new Container(x => x.For<Foobar>().Use<Foobar>().OnCreate.SetProperty("ResolvableDependency"));
			var instance = container.Get<Foobar>();

			instance.ResolvableDependency.Should().NotBeNull();
		}

		[Fact]
		public void set_missing_property_should_throw()
		{
			var container = new Container(x => x.For<Foobar>().Use<Foobar>().OnCreate.SetProperty("missing property"));

			container.Invoking(x => x.Get<Foobar>()).ShouldThrow<ActivationException>();
		}

		[Fact]
		public void set_property_with_unresolvable_type_should_throw()
		{
			var container = new Container(x => x.For<Foobar>().Use<Foobar>().OnCreate.SetProperty(y => y.UnresolvableDependency));

			container.Invoking(x => x.Get<Foobar>()).ShouldThrow<ActivationException>();
		}

		[Fact]
		public void try_set_property_with_unresolvable_type_should_work()
		{
			var container = new Container(x => x.For<Foobar>().Use<Foobar>().OnCreate.TrySetProperty(y => y.UnresolvableDependency));

			container.Invoking(x => x.Get<Foobar>()).ShouldNotThrow();
		}

		[Fact]
		public void try_set_missing_property_should_throw()
		{
			var container = new Container(x => x.For<Foobar>().Use<Foobar>().OnCreate.TrySetProperty("missing property"));

			container.Invoking(x => x.Get<Foobar>()).ShouldThrow<ActivationException>();
		}

		private class Foobar
		{
			public object ResolvableDependency { get; set; }
			public IDisposable UnresolvableDependency { get; set; }
		}
	}
}