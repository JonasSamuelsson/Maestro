using FluentAssertions;
using Maestro.Tests.Conventions.DefaultImplementationConvention;
using Xunit;

namespace Maestro.Tests.Conventions
{
	public class use_default_implementation_convention
	{
		[Fact]
		public void should_register_default_implementations()
		{
			var ns = typeof(IFoobar1).Namespace;

			var container = new Container(x => x.Scan.AssemblyContainingTypeOf(this).Where(y => y.Namespace != null && y.Namespace.StartsWith(ns)).UseDefaultImplementations());

			container.Invoking(x => x.GetService<IFoobar1>()).ShouldNotThrow();
			container.Invoking(x => x.GetService<IFoobar2>()).ShouldNotThrow();
			container.Invoking(x => x.GetService<IFoobar3>()).ShouldThrow<ActivationException>();
			container.Invoking(x => x.GetService<IFoobar4>()).ShouldThrow<ActivationException>();
		}

		[Fact]
		public void should_support_instance_configuration()
		{
			var container = new Container(x => x.Scan.Types(new[] { typeof(IFoobar1), typeof(Foobar1) }).UseDefaultImplementations(y => y.Lifetime.Singleton()));

			var instance1 = container.GetService<IFoobar1>();
			var instance2 = container.GetService<IFoobar1>();

			instance1.Should().Be(instance2);
		}
	}
}

namespace Maestro.Tests.Conventions.DefaultImplementationConvention
{
	internal interface IFoobar1 { }
	internal interface IFoobar2 { }
	internal interface IFoobar3 { }
	internal interface IFoobar4 { }

	internal class Foobar1 : IFoobar1 { }
	internal class Foobar2 : IFoobar2 { }
	internal class FooBar3 : IFoobar3 { }
}

namespace Maestro.Tests.Conventions.DefaultImplementationConvention.SubNamespace
{
	internal class Foobar4 : IFoobar4 { }
}