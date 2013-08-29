using FluentAssertions;
using Maestro.Lifetimes;
using Xunit;

namespace Maestro.Tests
{
	public class custom_lifetime
	{
		[Fact]
		public void provided_lifetime_should_be_executed()
		{
			var lifetime = new Lifetime();
			var container = new Container(x => x.For<object>().Use<object>().Lifetime.Custom(lifetime));

			container.Get<object>();

			lifetime.Executed.Should().BeTrue();
		}

		private class Lifetime : ILifetime
		{
			public bool Executed { get; private set; }

			public ILifetime Clone()
			{
				throw new System.NotImplementedException();
			}

			public object Execute(IContext context, IPipeline pipeline)
			{
				Executed = true;
				return pipeline.Execute();
			}
		}
	}
}