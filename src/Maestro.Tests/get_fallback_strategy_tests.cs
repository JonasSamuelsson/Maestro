using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class get_fallback_strategy_tests
	{
		[Fact]
		public void should_fallback_to_default_instance_if_named_instance_is_missing()
		{
			var container = new Container(x =>
			{
				x.Service<object>().Use.Instance("success");
			});

			container.Get<object>("test").ShouldBe("success");
		}

		[Fact(Skip = "feature missing")]
		public void should_fallback_to_type_provider() { }

		[Fact]
		public void should_fallback_to_instantiating_concrete_closed_classes()
		{
			new Container().Get<Foo<string>>().ShouldNotBeNull();
		}

		[Fact]
		public void should_prioritize_fallback_options()
		{
			var parentContainer = new Container();
			var childContainer = parentContainer.GetChildContainer();

			childContainer.Get<Foo<string>>("test").ShouldNotBeNull();

			// parent type provider

			// child type provider

			parentContainer.Configure(x => x.Service<Foo<string>>().Use.Type<Foo<string>>().SetProperty(y => y.Value, "parent"));
			childContainer.Get<Foo<string>>("test").Value.ShouldBe("parent");

			childContainer.Configure(x => x.Service<Foo<string>>().Use.Type<Foo<string>>().SetProperty(y => y.Value, "child"));
			childContainer.Get<Foo<string>>("test").Value.ShouldBe("child");
		}

		class Foo<T>
		{
			public T Value { get; set; }
		}
	}
}