using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Providers
{
	public class type_instance_provider_constructor_dependency_resolution
	{
		[Fact]
		public void should_resolve_registered_reference_type()
		{
			var @object = new object();
			var container = new Container(x => x.For<object>().Use(@object));
			var instance = container.Get<TypeWithOptionalConstructorDependency<object>>();
			instance.Dependency.Should().Be(@object);
		}

		[Fact]
		public void should_resolve_unregistered_but_instantiatable_reference_type()
		{
			var container = new Container();
			var instance = container.Get<TypeWithOptionalConstructorDependency<object>>();
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
			var container = new Container(x => x.For<int>().Use(@int));
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
			var @objects = new[] { new object(), new object() };
			var container = new Container(x => x.For<IEnumerable<object>>().Use(@objects));
			var instance = container.Get<TypeWithOptionalConstructorDependency<IEnumerable<object>>>();
			instance.Dependency.Should().BeEquivalentTo(@objects);
		}

		[Fact]
		public void should_resolve_reference_type_enumerable_with_registered_item()
		{
			var @object = new object();
			var container = new Container(x => x.For<object>().Use(@object));
			var instance = container.Get<TypeWithOptionalConstructorDependency<IEnumerable<object>>>();
			instance.Dependency.Should().BeEquivalentTo(new[] { @object });
		}

		[Fact]
		public void should_resolve_unregistered_reference_type_enumerable()
		{
			var container = new Container();
			var instance = container.Get<TypeWithOptionalConstructorDependency<IEnumerable<object>>>();
			instance.Dependency.Should().BeEmpty();
		}

		[Fact]
		public void should_resolve_registered_reference_type_array()
		{
			var @objects = new[] { new object(), new object() };
			var container = new Container(x => x.For<object[]>().Use(@objects));
			var instance = container.Get<TypeWithOptionalConstructorDependency<object[]>>();
			instance.Dependency.Should().BeEquivalentTo(@objects);
		}

		[Fact]
		public void should_resolve_reference_type_array_with_registered_item()
		{
			var @object = new object();
			var container = new Container(x => x.For<object>().Use(@object));
			var instance = container.Get<TypeWithOptionalConstructorDependency<object[]>>();
			instance.Dependency.Should().BeEquivalentTo(new[] { @object });
		}

		[Fact]
		public void should_resolve_unregistered_reference_type_array()
		{
			var container = new Container();
			var instance = container.Get<TypeWithOptionalConstructorDependency<object[]>>();
			instance.Dependency.Should().BeEmpty();
		}

		[Fact]
		public void should_resolve_registered_value_type_enumerable()
		{
			var ints = new[] { 1, 2, 3 };
			var container = new Container(x => x.For<IEnumerable<int>>().Use(ints));
			var instance = container.Get<TypeWithOptionalConstructorDependency<IEnumerable<int>>>();
			instance.Dependency.Should().BeEquivalentTo(ints);
		}

		[Fact]
		public void should_not_resolve_value_type_enumerable_with_registered_item()
		{
			var @int = 159;
			var container = new Container(x => x.For<int>().Use(@int));
			var instance = container.Get<TypeWithOptionalConstructorDependency<IEnumerable<int>>>();
			instance.Dependency.Should().BeNull();
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
			var container = new Container(x => x.For<int[]>().Use(ints));
			var instance = container.Get<TypeWithOptionalConstructorDependency<int[]>>();
			instance.Dependency.Should().BeEquivalentTo(ints);
		}

		[Fact]
		public void should_not_resolve_value_type_array_with_registered_item()
		{
			var @int = 159;
			var container = new Container(x => x.For<int>().Use(@int));
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

		private interface IReferenceType1 { }
		private interface IReferenceType2 { }
		private interface IReferenceType3 { }
		private class ReferenceType : IReferenceType1, IReferenceType2, IReferenceType3 { }
	}
}