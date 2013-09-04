using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Providers
{
	public class type_instance_provider
	{
		[Fact]
		public void should_use_constructor_with_most_resolvable_parameters()
		{
			var container = new Container(x =>
													{
														x.For<object>().Use<object>();
														x.For<TypeWithOptionalObjectDependency>().Use<TypeWithOptionalObjectDependency>();
														x.For<TypeWithOptionalStringDependency>().Use<TypeWithOptionalStringDependency>();
													});

			container.Get<TypeWithOptionalObjectDependency>().Object.Should().NotBeNull();
			container.Get<TypeWithOptionalStringDependency>().String.Should().BeNull();
		}

		[Fact]
		public void should_reevaluate_constructor_to_use_when_config_changes()
		{
			var @string = "string";
			var container = new Container(x => x.For<TypeWithOptionalStringDependency>().Use<TypeWithOptionalStringDependency>());

			container.Get<TypeWithOptionalStringDependency>().String.Should().BeNull();

			container.Configure(x => x.For<string>().Use(@string));

			container.Get<TypeWithOptionalStringDependency>().String.Should().Be(@string);
		}

		[Fact]
		public void should_instantiate_open_generic_type()
		{
			var container = new Container(x => x.For(typeof(GenericType<>)).Use(typeof(GenericType<>)));

			var instance = container.Get<GenericType<int>>();

			instance.Should().BeOfType<GenericType<int>>();
		}

		private class TypeWithOptionalObjectDependency
		{
			public TypeWithOptionalObjectDependency() { }

			public TypeWithOptionalObjectDependency(object @object)
			{
				Object = @object;
			}

			public object Object { get; private set; }
		}

		private class TypeWithOptionalStringDependency
		{
			public TypeWithOptionalStringDependency() { }

			public TypeWithOptionalStringDependency(string @string)
			{
				String = @string;
			}

			public object String { get; private set; }
		}

		private class GenericType<T> { }
	}
}