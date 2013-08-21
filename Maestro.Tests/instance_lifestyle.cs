using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class instance_lifestyle
	{
		[Fact]
		public void provided_lifestyle_should_be_executed()
		{
			var lifecycle = new Lifecycle();
			var container = new Container(x => x.For<object>().Use<object>().Lifecycle.Custom(lifecycle));

			container.Get<object>();

			lifecycle.Executed.Should().BeTrue();
		}

		private class Lifecycle : ILifecycle
		{
			public bool Executed { get; private set; }

			public object Process(IContext context, IPipeline pipeline)
			{
				Executed = true;
				return pipeline.Execute();
			}
		}
	}
}