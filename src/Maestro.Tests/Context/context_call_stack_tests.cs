using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Context
{
	public class context_call_stack_tests
	{
		[Fact]
		public void context_should_expose_call_stack()
		{
			var names = new List<string>();
			var types = new List<Type>();

			var container = new Container(x =>
			{
				x.Use<A>("foo").Factory(ctx => new A(ctx.GetService<B>("bar")));
				x.Use<B>("bar").Factory(ctx =>
				{
					names = ctx.CallStack.Select(y => y.Name).ToList();
					types = ctx.CallStack.Select(y => y.Type).ToList();
					return new B();
				});
			});

			container.GetService<A>("foo");

			names.ShouldBe(new[] { "bar", "foo" });
			types.ShouldBe(new[] { typeof(B), typeof(A) });
		}

		class A { public A(B b) { } }
		class B { }
	}
}