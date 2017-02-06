using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Interception
{
	public class intercept_instance
	{
		[Fact]
		public void interceptors_should_be_executed_in_the_same_order_they_are_configured()
		{
			var list = new List<string>();

			new Container(x => x.For<object>().Use.Self()
				.Intercept(_ => list.Add("one"))
				.Intercept(_ => list.Add("two"))
				.Intercept(_ => list.Add("three")))
			.GetService<object>();

			list.ShouldBe(new[] { "one", "two", "three" });
		}

		[Fact]
		public void interceptors_should_not_be_executed_if_instance_is_chached()
		{
			var counter = 0;

			var container = new Container(x => x.For<object>().Use.Self()
				.Intercept(_ => counter++)
				.Lifetime.Singleton());

			container.GetService<object>();
			counter.ShouldBe(1);
			container.GetService<object>();
			counter.ShouldBe(1);
		}

		[Todo]
		public void dynamic_proxy_interception()
		{
			var container = new Container(x => x.For<FooOnly>().Use.Self().Intercept());
		}

		class Foo
		{
			public string Foo { get; set; }
		}

		class FooAndBar : FooOnly
		{
			public string Bar { get; set; }
		}
	}
}