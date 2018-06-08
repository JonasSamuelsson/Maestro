using Shouldly;
using Xunit;

namespace Maestro.Tests.Conventions
{
	public class default_implementations_tests
	{
		[Fact]
		public void should_use_default_implementations()
		{
			var @namespace = typeof(IFoobar1).Namespace;

			var container = new Container(x => x.Scan(_ =>
			{
				_.AssemblyContainingTypeOf(this);
				_.Where(y => y.Namespace == @namespace);
				_.RegisterDefaultImplementations();
			}));

			container.GetService<IFoobar1>().ShouldBeOfType<Foobar1>();
		}

		[Fact]
		public void should_support_instance_configuration()
		{
			var container = new Container(x => x.Scan(_ =>
			{
				_.Types(new[] { typeof(IFoobar1), typeof(Foobar1) });
				_.RegisterDefaultImplementations(z => z.Use().Lifetime.Singleton());
			}));

			var instance1 = container.GetService<IFoobar1>();
			var instance2 = container.GetService<IFoobar1>();

			instance1.ShouldBe(instance2);
		}

		internal interface IFoobar1 { }
		internal class Foobar1 : IFoobar1 { }
	}
}