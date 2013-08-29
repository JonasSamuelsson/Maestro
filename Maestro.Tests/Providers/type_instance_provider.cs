using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Providers
{
	public class type_instance_provider
	{
		[Fact]
		public void should_instantiate_type_with_default_constructor()
		{
			var container = new Container(x => x.For<object>().Use<object>());

			var instance = container.Get<object>();

			instance.Should().NotBeNull();
		}

		[Fact]
		public void should_instantiate_type_with_constructor_dependency()
		{
			var container = new Container(x => x.For<TypeWithSingleCtorDependency>().Use<TypeWithSingleCtorDependency>());

			var instance = container.Get<TypeWithSingleCtorDependency>();

			instance.Object.Should().NotBeNull();
		}

		[Fact]
		public void should_use_constructor_with_most_resolvable_parameters()
		{
			var container = new Container(x => x.For<TypeWithMultipleCtors>().Use<TypeWithMultipleCtors>());

			var instance = container.Get<TypeWithMultipleCtors>();

			instance.Object.Should().NotBeNull();
			instance.String.Should().BeNull();
		}

		[Fact]
		public void should_reevaluate_constructor_to_use_when_config_changes()
		{
			var @string = "string";
			var container = new Container(x => x.For<TypeWithMultipleCtors>().Use<TypeWithMultipleCtors>());

			var instance1 = container.Get<TypeWithMultipleCtors>();

			instance1.Object.Should().NotBeNull();
			instance1.String.Should().BeNull();

			container.Configure(x => x.For<string>().Use(@string));
			var instance2 = container.Get<TypeWithMultipleCtors>();

			instance2.Object.Should().NotBeNull();
			instance2.String.Should().Be(@string);
		}

		[Fact]
		public void should_instantiate_open_generic_type()
		{
			var container = new Container(x => x.For(typeof(GenericType<>)).Use(typeof(GenericType<>)));

			var instance = container.Get<GenericType<int>>();

			instance.Should().BeOfType<GenericType<int>>();
		}

		private class TypeWithSingleCtorDependency
		{
			public TypeWithSingleCtorDependency(object @object)
			{
				Object = @object;
			}

			public object Object { get; private set; }
		}

		private class TypeWithMultipleCtors
		{
			public TypeWithMultipleCtors() { }

			public TypeWithMultipleCtors(object @object)
			{
				Object = @object;
			}

			public TypeWithMultipleCtors(object @object, string @string)
				: this(@object)
			{
				String = @string;
			}

			public object Object { get; private set; }
			public string String { get; private set; }
		}

		private class GenericType<T> { }
	}
}