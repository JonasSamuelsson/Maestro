using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class get_unregistered_concrete_class
	{
		[Fact]
		public void should_be_able_to_instantiate_top_level_EventArgs()
		{
			Should.NotThrow(() => new Container().GetService<EventArgs>());
		}

		[Fact]
		public void should_be_able_to_instantiate_with_dependency()
		{
			new Container().GetService<Foobar>().Dependency.ShouldNotBeNull();
		}

		private class Foobar
		{
			public Foobar(EventArgs o)
			{
				Dependency = o;
			}

			public EventArgs Dependency { get; private set; }
		}
	}
}