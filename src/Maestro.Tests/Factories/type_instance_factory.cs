using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Maestro.Tests.Factories
{
	public class type_instance_factory
	{
		[Fact]
		public void should_instantiate_type_with_no_dependencies()
		{
			var container = new Container(x => x.Use<INoDependencies>().Type<NoDependencies>());
			container.GetService<INoDependencies>();
		}

		[Fact]
		public void should_instantiate_type_with_dependencies()
		{
			var container = new Container(x =>
			{
				x.Use<INoDependencies>().Type<NoDependencies>();
				x.Use<IOneDependency<INoDependencies>>().Type<RequiredDependency<INoDependencies>>();
			});

			var instance = container.GetService<IOneDependency<INoDependencies>>();

			instance.Dependency.ShouldNotBe(null);
		}

		[Fact]
		public void should_use_constructor_with_most_resolvable_dependencies()
		{
			var container = new Container(x =>
			{
				x.Use<INoDependencies>().Type<NoDependencies>();
				x.Use<IOneDependency<INoDependencies>>().Type<OptionalDependency<INoDependencies>>();
			});

			var instance = container.GetService<IOneDependency<INoDependencies>>();

			instance.Dependency.ShouldNotBe(null);
		}

		[Fact]
		public void should_reevaluate_constructor_to_use_when_config_changes()
		{
			var container = new Container(x => x.Use<IOneDependency<INoDependencies>>().Type<OptionalDependency<INoDependencies>>());
			var instance = container.GetService<IOneDependency<INoDependencies>>();
			instance.Dependency.ShouldBe(null);

			container.Configure(x => x.Use<INoDependencies>().Type<NoDependencies>());
			instance = container.GetService<IOneDependency<INoDependencies>>();
			instance.Dependency.ShouldNotBe(null);
		}

		[Fact]
		public void should_instantiate_open_generic_type()
		{
			var container = new Container(x => x.Use(typeof(INoDependencies<>)).Type(typeof(NoDependencies<>)));
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
			var container = new Container(x => x.Add<INoDependencies>().Instance(dependency));

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
														x.Use<IEnumerable<int>>().Instance(ints);
														x.Use<IEnumerable<string>>().Instance(strings);
													});

			var instanceWithInts = container.GetService<OptionalDependency<IEnumerable<int>>>();
			var instanceWithStrings = container.GetService<OptionalDependency<IEnumerable<string>>>();

			instanceWithInts.Dependency.ShouldBe(ints);
			instanceWithStrings.Dependency.ShouldBe(strings);
		}

		[Fact]
		public void should_use_custom_parameter_values()
		{
			var container = new Container(x =>
			{
				x.Use<string>().Instance("Object");

				x.Add<OptionalDependency<object>>().Self().CtorArg(typeof(object), "Object");
				x.Add<OptionalDependency<object>>().Self().CtorArg(typeof(object), () => "Object");
				x.Add<OptionalDependency<object>>().Self().CtorArg(typeof(object), ctx => ctx.GetService<string>());
				x.Add(typeof(OptionalDependency<>)).Self().CtorArg(typeof(object), "Object");

				x.Add<OptionalDependency<object>>().Self().CtorArg<object>("Object");
				x.Add<OptionalDependency<object>>().Self().CtorArg<object>(() => "Object");
				x.Add<OptionalDependency<object>>().Self().CtorArg<object>(ctx => ctx.GetService<string>());
				x.Add(typeof(OptionalDependency<>)).Self().CtorArg<object>("Object");
			});

			container.GetServices<OptionalDependency<object>>().ShouldAllBe(x => "Object".Equals(x.Dependency));
		}

		interface INoDependencies
		{ }

		class NoDependencies : INoDependencies { }

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

		class RequiredDependency<T> : OptionalDependency<T>
		{
			public RequiredDependency(T dependency) : base(dependency) { }
		}

		[Fact]
		public void should_not_try_to_invoke_static_constructor()
		{
			var container = new Container(x => x.Use<Unresolvable>().Self());

			Should.Throw<ActivationException>(() => container.GetService<Unresolvable>());
		}

		class Unresolvable
		{
			static Unresolvable() { }
			public Unresolvable(int i) { }
		}
	}
}