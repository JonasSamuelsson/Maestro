using System;
using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class get_unregistered_concrete_class
	{
		[Fact]
		public void should_be_able_to_instantiate_top_level_EventArgs()
		{
			Action act = () => new Container().Get<EventArgs>();
			act.ShouldNotThrow();
		}

		[Fact]
		public void should_be_able_to_instantiate_with_dependency()
		{
			Func<Foobar> func = () => new Container().Get<Foobar>();
			Action act = () => func();
			
			act.ShouldNotThrow();

			var foobar = func();
			foobar.Dependency.Should().BeOfType<EventArgs>();
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