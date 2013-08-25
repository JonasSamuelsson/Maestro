using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class custom_lifecycle
	{
		[Fact]
		public void provided_lifecycle_should_be_executed()
		{
			var lifecycle = new Lifecycle();
			var container = new Container(x => x.For<object>().Use<object>().Lifecycle.Custom(lifecycle));

			container.Get<object>();

			lifecycle.Executed.Should().BeTrue();
		}

		private class Lifecycle : LifecycleBase
		{
			public bool Executed { get; private set; }

			public override object Process(IContext context, IPipeline pipeline)
			{
				Executed = true;
				return pipeline.Execute();
			}
		}
	}
}