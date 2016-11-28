﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using Maestro.Utils;
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
			var instance = container.Get<TypeWithOptionalConstructorDependency<Dependency>>();
			instance.Dependency.Should().Be(dependency);
		}

		[Fact]
		public void should_resolve_unregistered_but_instantiatable_reference_type()
		{
			var container = new Container();
			var instance = container.Get<TypeWithOptionalConstructorDependency<Dependency>>();
			instance.Dependency.Should().NotBeNull();
		}

		[Fact]
		public void should_not_resolve_unregistered_and_uninstantiatable_reference_type()
		{
			var container = new Container();
			var instance = container.Get<TypeWithOptionalConstructorDependency<IDisposable>>();
			instance.Dependency.Should().BeNull();
		}

		[Fact]
		public void should_resolve_registered_value_type()
		{
			var @int = 987;
			var container = new Container(x => x.Service<int>().Use.Instance(@int));
			var instance = container.Get<TypeWithOptionalConstructorDependency<int>>();
			instance.Dependency.Should().Be(@int);
		}

		[Fact]
		public void should_not_resolve_unregistered_value_type()
		{
			var container = new Container();
			var instance = container.Get<TypeWithOptionalConstructorDependency<int>>();
			instance.Dependency.Should().Be(default(int));
		}

		[Fact]
		public void should_resolve_registered_reference_type_enumerable()
		{
			var dependencies = new[] { new Dependency1(), new Dependency1() };
			var container = new Container(x => x.Service<IEnumerable<Dependency>>().Use.Instance(dependencies));
			var instance = container.Get<TypeWithOptionalConstructorDependency<IEnumerable<Dependency>>>();
			instance.Dependency.Should().BeEquivalentTo(dependencies);
		}

		[Fact]
		public void should_resolve_reference_type_enumerable_with_registered_item()
		{
			var dependency1 = new Dependency1();
			var dependency2 = new Dependency2();
			var container = new Container(x =>
													{
														x.Service<Dependency>("1").Use.Instance(dependency1);
														x.Service<Dependency>("2").Use.Instance(dependency2);
													});
			var instance = container.Get<TypeWithOptionalConstructorDependency<IEnumerable<Dependency>>>();
			instance.Dependency.Should().BeEquivalentTo(new object[] { dependency1, dependency2 });
		}

		[Fact]
		public void should_resolve_unregistered_reference_type_enumerable()
		{
			var container = new Container();
			var instance = container.Get<TypeWithOptionalConstructorDependency<IEnumerable<Dependency>>>();
			instance.Dependency.Should().BeEmpty();
		}

		[Fact]
		public void should_resolve_registered_reference_type_array()
		{
			var dependencies = new[] { new Dependency(), new Dependency() };
			var container = new Container(x => x.Service<Dependency[]>().Use.Instance(dependencies));
			var instance = container.Get<TypeWithOptionalConstructorDependency<Dependency[]>>();
			instance.Dependency.Should().BeEquivalentTo(dependencies);
		}

		[Todo]
		public void should_resolve_reference_type_array_with_registered_item()
		{
			var dependency = new Dependency();
			var container = new Container(x => x.Service<Dependency>().Use.Instance(dependency));
			var instance = container.Get<TypeWithOptionalConstructorDependency<Dependency[]>>();
			instance.Dependency.Should().BeEquivalentTo(new[] { dependency });
		}

		[Todo]
		public void should_resolve_unregistered_reference_type_array()
		{
			var container = new Container();
			var instance = container.Get<TypeWithOptionalConstructorDependency<Dependency[]>>();
			instance.Dependency.Should().BeEmpty();
		}

		[Fact]
		public void should_resolve_registered_value_type_enumerable()
		{
			var ints = new[] { 1, 2, 3 };
			var container = new Container(x => x.Service<IEnumerable<int>>().Use.Instance(ints));
			var instance = container.Get<TypeWithOptionalConstructorDependency<IEnumerable<int>>>();
			instance.Dependency.Should().BeEquivalentTo(ints);
		}

		[Fact]
		public void should_not_resolve_unregistered_value_type_enumerable()
		{
			var container = new Container();
			var instance = container.Get<TypeWithOptionalConstructorDependency<IEnumerable<int>>>();
			instance.Dependency.Should().BeNull();
		}

		[Fact]
		public void should_resolve_registered_value_type_array()
		{
			var ints = new[] { 1, 2, 3 };
			var container = new Container(x => x.Service<int[]>().Use.Instance(ints));
			var instance = container.Get<TypeWithOptionalConstructorDependency<int[]>>();
			instance.Dependency.Should().BeEquivalentTo(ints);
		}

		[Fact]
		public void should_not_resolve_value_type_array_with_registered_item()
		{
			var @int = 159;
			var container = new Container(x => x.Service<int>().Use.Instance(@int));
			var instance = container.Get<TypeWithOptionalConstructorDependency<int[]>>();
			instance.Dependency.Should().BeNull();
		}

		[Fact]
		public void should_not_resolve_unregistered_value_type_array()
		{
			var container = new Container();
			var instance = container.Get<TypeWithOptionalConstructorDependency<int[]>>();
			instance.Dependency.Should().BeNull();
		}

		private class TypeWithOptionalConstructorDependency<T>
		{
			public TypeWithOptionalConstructorDependency()
			{
			}

			public TypeWithOptionalConstructorDependency(T dependency)
			{
				Dependency = dependency;
			}

			public T Dependency { get; private set; }
		}

		private class Dependency { }
		private class Dependency1 : Dependency { }
		private class Dependency2 : Dependency { }
	}
}