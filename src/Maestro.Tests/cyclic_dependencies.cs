using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class cyclic_dependencies
	{
		[Todo]
		public void Get_should_throw_ActivationException()
		{
			Should.Throw<ActivationException>(() => new Container().Get<Alpha>())
					.InnerException.Message.ShouldBe(@"Cyclic dependency.
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Alpha
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Delta
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Charlie
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Beta
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Alpha");
		}

		[Todo]
		public void GetAll_should_throw_ActivationException()
		{
			var container = new Container(x => x.For<Alpha>().Use<Alpha>());
			Should.Throw<ActivationException>(() => container.GetAll<Alpha>())
					.InnerException.Message.ShouldBe(@"Cyclic dependency.
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Alpha
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Delta
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Charlie
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Beta
  Maestro.Tests.resolve_type_with_cyclic_dependencies+Alpha");
		}

		private class Alpha { public Alpha(Beta beta) { } }
		private class Beta { public Beta(Charlie charlie) { } }
		private class Charlie { public Charlie(Delta delta) { } }
		private class Delta { public Delta(Alpha alpha) { } }

		[Todo]
		public void CanGet_should_throw()
		{
			var container = new Container(x => x.For<object>().Use<object>().Intercept((_, ctx) => ctx.CanGet<Alpha>()));

			container.Get<object>();
		}

		[Fact]
		public void Get_should_throw_ActivationException_when_instances_are_resolved_via_lambda_factories()
		{
			var container = new Container(x =>
													{
														x.For<A>().Use(ctx => new A { B = ctx.Get<B>() });
														x.For<B>().Use(ctx => new B { A = ctx.Get<A>() });
													});

			var exception = Should.Throw<ActivationException>(() => container.Get<A>());

			exception.Message.ShouldBe("Can't get default instance of type 'Maestro.Tests.cyclic_dependencies+A'.");
			exception.InnerException.ShouldBeOfType<InvalidOperationException>();
			exception.InnerException.Message.ShouldBe("Cyclic dependency, 'Maestro.Tests.cyclic_dependencies+A'.");
		}

		class A { public B B { get; set; } }
		class B { public A A { get; set; } }
	}
}