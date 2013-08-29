using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Providers
{
	public class conditional_instance_provider
	{
		[Fact]
		public void FactMethodName()
		{
			var defaultObject = "default";
			var namedObject = "named";

			var container = new Container(x => x.For<object>().UseConditional(y =>
			{
				y.If(z => z.Name != Container.DefaultName).Use(namedObject);
				y.Default.Use(defaultObject);
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