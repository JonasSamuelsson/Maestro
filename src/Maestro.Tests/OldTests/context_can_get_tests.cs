﻿using Shouldly;
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
														x.Service<A>().Use.Type<A>();
														x.Service<B>().Use.Instance(new B());
														x.Service<C>().Use.Factory(() => new C());
														x.Service<Instance>().Use.Factory(ctx => new Instance
														{
															Flags = new[]
																											 {
																												 ctx.CanGetService<A>(),
																												 ctx.CanGetService<B>(),
																												 ctx.CanGetService<C>()
																											 }
														});
													});

			var instance = container.GetService<Instance>();

			instance.Flags.ShouldBe(new[] { true, true, true });
		}

		[Fact]
		public void should_return_true_if_type_is_registered_but_cant_be_instantiated()
		{
			var container = new Container(x =>
													{
														x.Service<object>().Use.Type<object>();
														x.Service<Instance>().Use.Factory(ctx => new Instance { Flags = new[] { ctx.CanGetService<object>() } });
													});

			var instance = container.GetService<Instance>();

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