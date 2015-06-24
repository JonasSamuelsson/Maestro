﻿using System.Linq;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class open_generic_type
	{
		[Fact]
		public void should_get_generic_type()
		{
			var container = new Container(x => x.For(typeof(Instance<>)).Use(typeof(Instance<>)));

			var instance = container.Get<Instance<int>>();

			instance.ShouldBeOfType<Instance<int>>();
		}

		[Fact]
		public void should_execute_configured_interceptors()
		{
			var container = new Container(x =>
			{
				x.For<int>().Use(1);
				x.For<string>().Use("foobar");
				x.For(typeof(Instance<>)).Use(typeof(Instance<>)).SetProperty("Value");
			});

			var instance1 = container.Get<Instance<int>>();
			var instance2 = container.Get<Instance<string>>();

			instance1.Value.ShouldBe(1);
			instance2.Value.ShouldBe("foobar");
		}

		[Fact]
		public void should_use_configured_lifetime()
		{
			var container = new Container(x =>
			{
				x.For<int>().Use(1);
				x.For<string>().Use("foobar");
				x.For(typeof(Instance<>)).Use(typeof(Instance<>)).Lifetime.Singleton();
			});

			var instance1 = container.Get<Instance<int>>();
			var instance2 = container.Get<Instance<int>>();
			var instance3 = container.Get<Instance<string>>();
			var instance4 = container.Get<Instance<string>>();

			instance1.Value.ShouldBe(instance2.Value);
			instance3.Value.ShouldBe(instance4.Value);
		}

		[Fact]
		public void child_container_config_should_be_considered_over_parent_container()
		{
			var parent = new Container(x => x.For<Instance<string>>().Use(() => new Instance<string> { Value = "root" }));
			var child = parent.GetChildContainer(x => x.For(typeof(Instance<>)).Use(typeof(Instance<>)));

			var instance = child.Get<Instance<string>>();

			instance.Value.ShouldBe(null);
		}

		[Fact]
		public void get_all_should_handle_child_parent_container_configurations()
		{
			var container = new Container(x =>
													{
														x.For(typeof(Instance<>)).Use(typeof(Instance<>)).SetProperty("Value", "parent-default");
														x.For(typeof(Instance<>), "1").Use(typeof(Instance<>)).SetProperty("Value", "parent-1");
														x.For<Instance<string>>("2").Use<Instance<string>>().SetProperty("Value", "parent-2");
														x.For(typeof(Instance<>), "3").Use(typeof(Instance<>)).SetProperty("Value", "parent-3");
														x.For(typeof(Instance<>)).Add(typeof(Instance<>)).SetProperty("Value", "parent");
													})
				.GetChildContainer(x =>
										 {
											 x.For(typeof(Instance<>)).Use(typeof(Instance<>)).SetProperty("Value", "child-default");
											 x.For(typeof(Instance<>), "1").Use(typeof(Instance<>)).SetProperty("Value", "child-1");
											 x.For(typeof(Instance<>), "2").Use(typeof(Instance<>)).SetProperty("Value", "child-2");
											 x.For<Instance<string>>("3").Use<Instance<string>>().SetProperty("Value", "child-3");
											 x.For(typeof(Instance<>)).Add(typeof(Instance<>)).SetProperty("Value", "child");
										 });

			var instances = container.GetAll<Instance<string>>().ToList();

			instances.Count.ShouldBe(6);
			instances[0].Value.ShouldBe("child-3");
			instances[1].Value.ShouldBe("child-default");
			instances[2].Value.ShouldBe("child-1");
			instances[3].Value.ShouldBe("child-2");
			instances[4].Value.ShouldBe("child");
			instances[5].Value.ShouldBe("parent");
		}

		[Fact]
		public void get_all_should_not_evaluate_open_generic_type_multiple_times_on_consecutive_calls()
		{
			var container = new Container(x => x.For(typeof(Instance<>)).Add(typeof(Instance<>)));

			var instances1 = container.GetAll<Instance<string>>();
			var instances2 = container.GetAll<Instance<string>>();

			instances1.Count().ShouldBe(instances2.Count());
		}

		class Instance<T>
		{
			public T Value { get; set; }
		}
	}
}