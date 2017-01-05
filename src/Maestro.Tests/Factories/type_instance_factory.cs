using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Factories
{
	public class type_instance_factory
	{
		[Fact]
		public void should_instantiate_type_with_no_dependencies()
		{
			var container = new Container(x => x.For<INoDependencies>().Use.Type<NoDependencies>());
			container.GetService<INoDependencies>();
		}

		[Fact]
		public void should_instantiate_type_with_dependencies()
		{
			var container = new Container(x =>
			{
				x.For<INoDependencies>().Use.Type<NoDependencies>();
				x.For<IOneDependency>().Use.Type<RequiredDependency>();
			});

			var instance = container.GetService<IOneDependency>();

			instance.Dependency.ShouldNotBe(null);
		}

		[Fact]
		public void should_use_constructor_with_most_resolvable_dependencies()
		{
			var container = new Container(x =>
			{
				x.For<INoDependencies>().Use.Type<NoDependencies>();
				x.For<IOneDependency>().Use.Type<OptionalDependency>();
			});

			var instance = container.GetService<IOneDependency>();

			instance.Dependency.ShouldNotBe(null);
		}

		[Fact]
		public void should_reevaluate_constructor_to_use_when_config_changes()
		{
			var container = new Container(x => x.For<IOneDependency>().Use.Type<OptionalDependency>());
			var instance = container.GetService<IOneDependency>();
			instance.Dependency.ShouldBe(null);

			container.Configure(x => x.For<INoDependencies>().Use.Type<NoDependencies>());
			instance = container.GetService<IOneDependency>();
			instance.Dependency.ShouldNotBe(null);
		}

		[Fact]
		public void should_instantiate_open_generic_type()
		{
			var container = new Container(x => x.For(typeof(INoDependencies<>)).Use.Type(typeof(NoDependencies<>)));
			container.GetService<INoDependencies<NoDependencies>>();
		}

		[Fact]
		public void should_get_type_with_empty_enumerable_dependency()
		{
			var container = new Container();

			var instance = container.GetService<OptionalDependency<IEnumerable<INoDependencies>>>();

			instance.Dependency.ShouldBeEmpty();
		}

		[Fact]
		public void should_get_type_with_enumerable_dependency()
		{
			var dependency = new NoDependencies();
			var container = new Container(x => x.For<INoDependencies>().Add.Instance(dependency));

			var instance = container.GetService<OptionalDependency<IEnumerable<INoDependencies>>>();

			instance.Dependency.ShouldBe(new INoDependencies[] { dependency });
		}

		[Fact]
		public void should_use_constructor_with_enumerable_struct_or_string_dependency_if_dependency_is_explicitly_configured()
		{
			var ints = new[] { 1, 2, 3 };
			var strings = new[] { "1", "2", "3" };
			var container = new Container(x =>
													{
														x.For<IEnumerable<int>>().Use.Instance(ints);
														x.For<IEnumerable<string>>().Use.Instance(strings);
													});

			var instanceWithInts = container.GetService<OptionalDependency<IEnumerable<int>>>();
			var instanceWithStrings = container.GetService<OptionalDependency<IEnumerable<string>>>();

			instanceWithInts.Dependency.ShouldBe(ints);
			instanceWithStrings.Dependency.ShouldBe(strings);
		}

		interface INoDependencies
		{ }

		interface IOneDependency
		{
			INoDependencies Dependency { get; }
		}

		class NoDependencies : INoDependencies { }
		class NoDependencies1 : NoDependencies { }
		class NoDependencies2 : NoDependencies { }

		class OptionalDependency : IOneDependency
		{
			public OptionalDependency() { }

			public OptionalDependency(INoDependencies dependency)
			{
				Dependency = dependency;
			}

			public INoDependencies Dependency { get; }
		}

		class RequiredDependency : OptionalDependency
		{
			public RequiredDependency(INoDependencies dependency) : base(dependency)
			{ }
		}

		interface INoDependencies<T>
		{ }

		class NoDependencies<T> : INoDependencies<T>
		{ }

		interface IOneDependency<T>
		{
			T Dependency { get; }
		}

		class OptionalDependency<T> : IOneDependency<T>
		{
			public OptionalDependency() { }

			public OptionalDependency(T dependency)
			{
				Dependency = dependency;
			}

			public T Dependency { get; }
		}
	}
}