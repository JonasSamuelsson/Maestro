using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Factories
{
	public class conditional_instance_factory
	{
		[Fact]
		public void should_get_predicated_or_fallback_instance()
		{
			var defaultObject = "default";
			var namedObject = "named";

			var container = new Container(x => x.For<object>().Use(y =>
			{
				y.If(z => z.Name != Container.DefaultName)
					.Use(namedObject);
				y.Else.Use(defaultObject);
			}));

			container.Get<Foobar>().Dependency.Should().Be(defaultObject);
			container.Get<Foobar>("not default").Dependency.Should().Be(namedObject);
		}

		private class Foobar
		{
			public Foobar(object dependency)
			{
				Dependency = dependency;
			}

			public object Dependency { get; private set; }
		}
	}
}