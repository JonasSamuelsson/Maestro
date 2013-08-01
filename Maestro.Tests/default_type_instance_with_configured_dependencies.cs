using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class default_type_instance_with_configured_dependencies
	{
		[Fact]
		public void should_get_instance_with_single_ctor_dependency()
		{
			var container = new Container(x =>
			{
				x.Default(typeof(TypeWithDefaultCtor)).Type(typeof(TypeWithDefaultCtor));
				x.Default(typeof(TypeWithCtorDependency)).Type(typeof(TypeWithCtorDependency));
			});

			var o = container.Get<TypeWithCtorDependency>();

			o.Should().NotBeNull();
			o.Dependency.Should().NotBeNull();
		}

		private class TypeWithDefaultCtor { }

		private class TypeWithCtorDependency
		{
			public TypeWithCtorDependency(TypeWithDefaultCtor dependency)
			{
				Dependency = dependency;
			}

			public TypeWithDefaultCtor Dependency { get; set; }
		}
	}
}