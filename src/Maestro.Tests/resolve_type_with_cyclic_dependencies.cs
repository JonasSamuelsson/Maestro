using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_type_with_cyclic_dependencies
	{
		[Fact]
		public void should_throw_ActivationException()
		{
			Should.Throw<ActivationException>(() => new Container().Get<Alpha>())
					.InnerException.Message.ShouldBe(@"Cyclic dependency.
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Beta
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Delta
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Charlie
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Beta
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Alpha");
		}

		private class Alpha { public Alpha(Beta beta) { } }
		private class Beta { public Beta(Charlie charlie) { } }
		private class Charlie { public Charlie(Delta delta) { } }
		private class Delta { public Delta(Beta beta) { } }
	}
}