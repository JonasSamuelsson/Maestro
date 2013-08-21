using FluentAssertions;
using Maestro.Tests.UseDefaultImplementationConvention;
using Xunit;

namespace Maestro.Tests
{
	public class use_default_implementation_convention
	{
		[Fact]
		public void should_register_default_implementations()
		{
			var ns = "Maestro.Tests.DefaultImplementationConvention";

			var container = new Container(x => x.Scan.AssemblyContainingTypeOf(this).Where(y => y.Namespace != null && y.Namespace.StartsWith(ns)).ForDefaultImplementations());

			container.Invoking(x => x.Get<IFoobar1>()).ShouldNotThrow();
			container.Invoking(x => x.Get<IFoobar2>()).ShouldNotThrow();
			container.Invoking(x => x.Get<IFoobar3>()).ShouldThrow<ActivationException>();
			container.Invoking(x => x.Get<IFoobar4>()).ShouldThrow<ActivationException>();
		}
	}
}

namespace Maestro.Tests.UseDefaultImplementationConvention
{
	internal interface IFoobar1 { }
	internal interface IFoobar2 { }
	internal interface IFoobar3 { }
	internal interface IFoobar4 { }

	internal class Foobar1 : IFoobar1 { }
	internal class Foobar2 : IFoobar2 { }
	internal class Foobar_3 : IFoobar3 { }
}

namespace Maestro.Tests.UseDefaultImplementationConvention.SubNamespace
{
	internal class Foobar4 : IFoobar4 { }
}