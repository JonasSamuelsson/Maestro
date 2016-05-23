using System.Collections.Generic;
using System.Collections.ObjectModel;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class ChildContainer_resolution_tests
	{
		[Fact]
		public void should_prefer_generic_child_container_config_over_specific_parent_config()
		{
			var parentContainer = new Container(x => x.Service<ICollection<object>>().Use.Type<Collection<object>>());
			var childContainer = parentContainer.GetChildContainer(x => x.Service(typeof(ICollection<>)).Use.Type(typeof(List<>)));

			var collection = childContainer.Get<ICollection<object>>();

			collection.ShouldBeOfType<List<object>>();
		}

		[Fact]
		public void GetAll_should_get_instances_from_parent_container_if_name_is_different()
		{
			var parentContainer = new Container(x =>
			{
				x.Service<object>().Use.Instance("default parent");
				x.Services<object>().Add.Instance("parent");
			});

			var childContainer = parentContainer.GetChildContainer(x =>
			{
				x.Service<object>().Use.Instance("default child");
				x.Services<object>().Add.Instance("child");
			});

			childContainer.GetAll<object>().ShouldBe(new[] { "default child", "child", "parent" });
		}
	}

	public class get_fallback_strategy_tests
	{
		[Fact]
		public void should_fallback_to_default_instance_if_named_instance_is_missing()
		{
			var container = new Container(x =>
			{
				x.For<object>().Use("success");
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

			parentContainer.Configure(x => x.For<Foo<string>>().Use<Foo<string>>().SetProperty(y => y.Value, "parent"));
			childContainer.Get<Foo<string>>("test").Value.ShouldBe("parent");

			childContainer.Configure(x => x.For<Foo<string>>().Use<Foo<string>>().SetProperty(y => y.Value, "child"));
			childContainer.Get<Foo<string>>("test").Value.ShouldBe("child");
		}

		class Foo<T>
		{
			public T Value { get; set; }
		}
	}
}
