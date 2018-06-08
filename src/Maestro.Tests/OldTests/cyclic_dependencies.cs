using Shouldly;
using Xunit;

namespace Maestro.Tests.OldTests
{
	public class cyclic_dependencies
	{
		[Todo]
		public void Get_should_throw_ActivationException()
		{
			Should.Throw<ActivationException>(() => new Container().GetService<Alpha>())
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
			var container = new Container(x => x.Use<Alpha>().Type<Alpha>());
			Should.Throw<ActivationException>(() => container.GetServices<Alpha>())
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
			var container = new Container(x => x.Use<object>().Type<object>().Intercept((_, ctx) => ctx.CanGetService<Alpha>()));

			container.GetService<object>();
		}

		[Fact]
		public void Get_should_throw_ActivationException_when_instances_are_resolved_via_lambda_factories()
		{
			var container = new Container(x =>
													{
														x.Use<A>().Factory(ctx => new A { B = ctx.GetService<B>() });
														x.Use<B>().Factory(ctx => new B { A = ctx.GetService<A>() });
													});

			Should.Throw<ActivationException>(() => container.GetService<A>()).Message.ShouldContain("cyclic dependency");

			// todo
			//exception.Message.ShouldBe("Can't get default instance of type 'Maestro.Tests.cyclic_dependencies+A'.");
			//exception.InnerException.ShouldBeOfType<InvalidOperationException>();
			//exception.InnerException.Message.ShouldBe("Cyclic dependency, 'Maestro.Tests.cyclic_dependencies+A'.");
		}

		class A { public B B { get; set; } }
		class B { public A A { get; set; } }
	}
}