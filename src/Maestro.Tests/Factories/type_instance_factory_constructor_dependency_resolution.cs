using System;
using System.Collections.Generic;
using Maestro.Utils;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Factories
{
	public abstract class type_instance_factory_constructor_dependency_resolution
	{
		public class type_instance_factory_constructor_dependency_resolution_using_expressions : type_instance_factory_constructor_dependency_resolution { }

		public class type_instance_factory_constructor_dependency_resolution_using_reflection : type_instance_factory_constructor_dependency_resolution, IDisposable
		{
			public type_instance_factory_constructor_dependency_resolution_using_reflection()
			{
				Reflector.AlwaysUseReflection = true;
			}

			public void Dispose()
			{
				Reflector.AlwaysUseReflection = false;
			}
		}

		[Fact]
		public void should_resolve_registered_reference_type()
		{
			var dependency = new Dependency();
			var container = new Container(x => x.Service<Dependency>().Use.Instance(dependency));
			var instance = container.GetService<TypeWithOptionalConstructorDependency<Dependency>>();
			instance.Dependency.ShouldBe(dependency);
		}

		[Fact]
		public void should_resolve_unregistered_but_instantiatable_reference_type()
		{
			var container = new Container();
			var instance = container.GetService<TypeWithOptionalConstructorDependency<Dependency>>();
			instance.Dependency.ShouldNotBeNull();
		}

		[Fact]
		public void should_not_resolve_unregistered_and_uninstantiatable_reference_type()
		{
			var container = new Container();
			var instance = container.GetService<TypeWithOptionalConstructorDependency<IDisposable>>();
			instance.Dependency.ShouldBeNull();
		}

		[Fact]
		public void should_resolve_registered_value_type()
		{
			var @int = 987;
			var container = new Container(x => x.Service<int>().Use.Instance(@int));
			var instance = container.GetService<TypeWithOptionalConstructorDependency<int>>();
			instance.Dependency.ShouldBe(@int);
		}

		[Fact]
		public void should_not_resolve_unregistered_value_type()
		{
			var container = new Container();
			var instance = container.GetService<TypeWithOptionalConstructorDependency<int>>();
			instance.Dependency.ShouldBe(default(int));
		}

		[Fact]
		public void should_resolve_registered_reference_type_enumerable()
		{
			var dependencies = new[] { new Dependency1(), new Dependency1() };
			var container = new Container(x => x.Service<IEnumerable<Dependency>>().Use.Instance(dependencies));
			var instance = container.GetService<TypeWithOptionalConstructorDependency<IEnumerable<Dependency>>>();
			instance.Dependency.ShouldBe(dependencies);
		}

		[Fact]
		public void should_resolve_reference_type_enumerable_with_registered_item()
		{
			var dependency1 = new Dependency1();
			var dependency2 = new Dependency2();
			var container = new Container(x =>
													{
														x.Services<Dependency>().Add.Instance(dependency1);
														x.Services<Dependency>().Add.Instance(dependency2);
													});
			var instance = container.GetService<TypeWithOptionalConstructorDependency<IEnumerable<Dependency>>>();
			instance.Dependency.ShouldBe(new Dependency[] { dependency1, dependency2 });
		}

		[Fact]
		public void should_resolve_unregistered_reference_type_enumerable()
		{
			var container = new Container();
			var instance = container.GetService<TypeWithOptionalConstructorDependency<IEnumerable<Dependency>>>();
			instance.Dependency.ShouldBeEmpty();
		}

		[Fact]
		public void should_resolve_registered_reference_type_array()
		{
			var dependencies = new[] { new Dependency(), new Dependency() };
			var container = new Container(x => x.Service<Dependency[]>().Use.Instance(dependencies));
			var instance = container.GetService<TypeWithOptionalConstructorDependency<Dependency[]>>();
			instance.Dependency.ShouldBe(dependencies);
		}

		[Todo]
		public void should_resolve_reference_type_array_with_registered_item()
		{
			var dependency = new Dependency();
			var container = new Container(x => x.Service<Dependency>().Use.Instance(dependency));
			var instance = container.GetService<TypeWithOptionalConstructorDependency<Dependency[]>>();
			instance.Dependency.ShouldBe(new[] { dependency });
		}

		[Todo]
		public void should_resolve_unregistered_reference_type_array()
		{
			var container = new Container();
			var instance = container.GetService<TypeWithOptionalConstructorDependency<Dependency[]>>();
			instance.Dependency.ShouldBeEmpty();
		}

		[Fact]
		public void should_resolve_registered_value_type_enumerable()
		{
			var ints = new[] { 1, 2, 3 };
			var container = new Container(x => x.Service<IEnumerable<int>>().Use.Instance(ints));
			var instance = container.GetService<TypeWithOptionalConstructorDependency<IEnumerable<int>>>();
			instance.Dependency.ShouldBe(ints);
		}

		[Fact]
		public void should_not_resolve_unregistered_value_type_enumerable()
		{
			var container = new Container();
			var instance = container.GetService<TypeWithOptionalConstructorDependency<IEnumerable<int>>>();
			instance.Dependency.ShouldBeNull();
		}

		[Fact]
		public void should_resolve_registered_value_type_array()
		{
			var ints = new[] { 1, 2, 3 };
			var container = new Container(x => x.Service<int[]>().Use.Instance(ints));
			var instance = container.GetService<TypeWithOptionalConstructorDependency<int[]>>();
			instance.Dependency.ShouldBe(ints);
		}

		[Fact]
		public void should_not_resolve_value_type_array_with_registered_item()
		{
			var @int = 159;
			var container = new Container(x => x.Service<int>().Use.Instance(@int));
			var instance = container.GetService<TypeWithOptionalConstructorDependency<int[]>>();
			instance.Dependency.ShouldBeNull();
		}

		[Fact]
		public void should_not_resolve_unregistered_value_type_array()
		{
			var container = new Container();
			var instance = container.GetService<TypeWithOptionalConstructorDependency<int[]>>();
			instance.Dependency.ShouldBeNull();
		}

		// ReSharper disable once ClassNeverInstantiated.Local
		private class TypeWithOptionalConstructorDependency<T>
		{
			// ReSharper disable once UnusedMember.Local
			public TypeWithOptionalConstructorDependency()
			{
			}

			// ReSharper disable once UnusedMember.Local
			public TypeWithOptionalConstructorDependency(T dependency)
			{
				Dependency = dependency;
			}

			// ReSharper disable once MemberHidesStaticFromOuterClass
			public T Dependency { get; }
		}

		private class Dependency { }
		private class Dependency1 : Dependency { }
		private class Dependency2 : Dependency { }
	}
}