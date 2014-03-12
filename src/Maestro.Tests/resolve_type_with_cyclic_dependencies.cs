using FluentAssertions;
using System;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_type_with_cyclic_dependencies
	{
		[Fact]
		public void should_throw_ActivationException()
		{
			Action act = () => new Container().Get<Foo>();
			act.ShouldThrow<ActivationException>();
		}

		private class Foo { public Foo(Bar bar) { } }
		private class Bar { public Bar(Foo foo) { } }
	}
}