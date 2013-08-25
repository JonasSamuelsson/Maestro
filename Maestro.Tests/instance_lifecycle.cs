using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class instance_lifecycle
	{
		[Fact]
		public void provided_lifestyle_should_be_executed()
		{
			var lifecycle = new Lifecycle();
			var container = new Container(x => x.For<object>().Use<object>().Lifecycle.Custom(lifecycle));

			container.Get<object>();

			lifecycle.Executed.Should().BeTrue();
		}

		[Fact]
		public void sinlgeton_instances_should_always_return_the_same_instance()
		{
			var container = new Container(x => x.For<object>().Use<object>().Lifecycle.Singleton());

			var o1 = container.Get<object>();
			var o2 = container.Get<object>();

			o1.Should().Be(o2);
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