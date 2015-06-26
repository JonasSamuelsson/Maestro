using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class context_can_get_tests
	{
		[Fact]
		public void should_return_true_if_type_is_registered()
		{
			var container = new Container(x =>
													{
														x.For<A>().Use<A>();
														x.For<B>().Use(new B());
														x.For<C>().Use(() => new C());
														x.For<Instance>().Use(ctx => new Instance
														{
															Flags = new[]
																											 {
																												 ctx.CanGet<A>(),
																												 ctx.CanGet<B>(),
																												 ctx.CanGet<C>()
																											 }
														});
													});

			var instance = container.Get<Instance>();

			instance.Flags.ShouldBe(new[] { true, true, true });
		}

		[Fact]
		public void should_return_true_if_type_is_registered_but_cant_be_instantiated()
		{
			var container = new Container(x =>
													{
														x.For<object>().Use<object>(ctx => { throw new NotImplementedException(); });
														x.For<Instance>().Use(ctx => new Instance { Flags = new[] { ctx.CanGet<object>() } });
													});

			var instance = container.Get<Instance>();

			instance.Flags.ShouldBe(new[] { true });
		}

		class Instance
		{
			public bool[] Flags { get; set; }
		}

		class A { }
		class B { }
		class C { }
	}
}