using Shouldly;
using Xunit;

namespace Maestro.Tests.Core
{
	public class get_fallback_strategy_tests
	{
		[Fact]
		public void should_fallback_to_default_instance_if_named_instance_is_missing()
		{
			var container = new Container(x =>
			{
				x.Use<object>().Instance("success");
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

		[Fact]
		public void should_prioritize_fallback_options()
		{
			var parentContainer = new Container();
			var childContainer = parentContainer.GetChildContainer();

			childContainer.GetService<Foo<string>>("test").ShouldNotBeNull();

			// parent type provider

			// child type provider

			parentContainer.Configure(x => x.Use<Foo<string>>().Type<Foo<string>>().SetProperty(y => y.Value, "parent"));
			childContainer.GetService<Foo<string>>("test").Value.ShouldBe("parent");

			childContainer.Configure(x => x.Use<Foo<string>>().Type<Foo<string>>().SetProperty(y => y.Value, "child"));
			childContainer.GetService<Foo<string>>("test").Value.ShouldBe("child");
		}

		class Foo<T>
		{
			public T Value { get; set; }
		}
	}
}