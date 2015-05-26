using System;
using FluentAssertions;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Factories
{
	public class type_instance_factory
	{
		[Fact]
		public void should_instantiate_type_without_dependencies()
		{
			var container = new Container(x => x.For<IZeroDependencies>().Use<ZeroDependencies>());
			container.Get<IZeroDependencies>();
		}

		[Fact]
		public void should_instantiate_type_with_dependencies()
		{
			var container = new Container(x =>
			{
				x.For<IZeroDependencies>().Use<ZeroDependencies>();
				x.For<IOneDependency>().Use<OneRequiredDependency>();
			});

			var instance = container.Get<IOneDependency>();

			instance.Dependency.ShouldNotBe(null);
		}

		[Fact]
		public void should_use_constructor_with_most_resolvable_dependencies()
		{
			var container = new Container(x =>
			{
				x.For<IZeroDependencies>().Use<ZeroDependencies>();
				x.For<IOneDependency>().Use<OneOptionalDependency>();
			});

			var instance = container.Get<IOneDependency>();

			instance.Dependency.ShouldNotBe(null);
		}

		[Fact]
		public void should_reevaluate_constructor_to_use_when_config_changes()
		{
			var container = new Container(x => x.For<IOneDependency>().Use<OneOptionalDependency>());
			var instance = container.Get<IOneDependency>();
			instance.Dependency.ShouldBe(null);

			container.Configure(x => x.For<IZeroDependencies>().Use<ZeroDependencies>());
			instance = container.Get<IOneDependency>();
			instance.Dependency.ShouldNotBe(null);
		}

		[Fact]
		public void should_instantiate_open_generic_type()
		{
			var container = new Container(x => x.For(typeof(IZeroDependencies<>)).Use(typeof(ZeroDependencies)));
			container.Get<IZeroDependencies<object>>();
		}

		interface IZeroDependencies
		{
		}

		interface IOneDependency
		{
			IZeroDependencies Dependency { get; }
		}

		class ZeroDependencies : IZeroDependencies
		{ }

		class OneOptionalDependency : IOneDependency
		{
			public OneOptionalDependency() { }

			public OneOptionalDependency(IZeroDependencies dependency)
			{
				Dependency = dependency;
			}

			public IZeroDependencies Dependency { get; }
		}

		class OneRequiredDependency : OneOptionalDependency
		{
			public OneRequiredDependency(IZeroDependencies dependency) : base(dependency)
			{
			}
		}

		interface IZeroDependencies<T>
		{ }

		class ZeroDependencies<T> : IZeroDependencies<T>
		{ }
	}
}