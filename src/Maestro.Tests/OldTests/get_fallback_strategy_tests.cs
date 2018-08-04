using Shouldly;
using Xunit;

namespace Maestro.Tests.OldTests
{
	public class get_fallback_strategy_tests
	{
		[Fact]
		public void should_fallback_to_default_instance_if_named_instance_is_missing()
		{
			var container = new Container(x =>
			{
				x.Add<object>().Instance("success");
			});

			container.GetService<object>("test").ShouldBe("success");
		}

		[Fact(Skip = "feature missing")]
		public void should_fallback_to_type_provider() { }

		[Fact]
		public void should_fallback_to_instantiating_concrete_closed_classes()
		{
			new Container().GetService<Foo<string>>().ShouldNotBeNull();
		}

		class Foo<T>
		{
			public T Value { get; set; }
		}
	}
}