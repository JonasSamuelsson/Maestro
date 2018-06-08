using Maestro.Interceptors;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Interception
{
	public class interception_tests
	{
		[Fact]
		public void should_execute_provided_interceptor()
		{
			var i = new Instance();
			var container = new Container(x =>
			{
				x.Use<Instance>().Factory(() => i).Intercept(new InstanceInterceptor());
			});

			var instance = container.GetService<Instance>();

			instance.ShouldNotBe(i);
			instance.InnerInstance.ShouldBe(i);
		}

		class Instance
		{
			public Instance() { }

			public Instance(Instance innerInstance)
			{
				InnerInstance = innerInstance;
			}

			public Instance InnerInstance { get; }
		}

		class InstanceInterceptor : Interceptor<Instance>
		{
			public override Instance Execute(Instance instance, Maestro.Context context)
			{
				return new Instance(instance);
			}
		}

		[Fact]
		public void should_execute_provided_action()
		{
			var container = new Container(x =>
			{
				x.Use<Wrapper<int>>("1").Type<Wrapper<int>>().Intercept(instance => instance.Value = 1);
				x.Use<Wrapper<int>>("2").Type<Wrapper<int>>().Intercept((instance, ctx) => instance.Value = 2);
			});

			container.GetService<Wrapper<int>>("1").Value.ShouldBe(1);
			container.GetService<Wrapper<int>>("2").Value.ShouldBe(2);
		}

		[Fact]
		public void should_execute_provided_func()
		{
			var parent = new Parent();
			var container = new Container(x =>
			{
				x.Use<Parent>("1").Factory(() => parent).Intercept(instance => new Child { Parent = instance });
				x.Use<Parent>("2").Factory(() => parent).Intercept((instance, ctx) => new Child { Parent = instance });
			});

			container.GetService<Parent>("1").ShouldBeOfType<Child>().Parent.ShouldBe(parent);
			container.GetService<Parent>("2").ShouldBeOfType<Child>().Parent.ShouldBe(parent);
		}

		[Fact]
		public void interceptors_should_be_executed_in_the_same_order_as_they_are_configured()
		{
			var container = new Container(x => x.Use<Wrapper<string>>().Type<Wrapper<string>>()
				.Intercept(y => y.Value += 1)
				.Intercept(y => y.Value += 2)
				.Intercept(new StringWrapperInterceptor())
				.Intercept(y => y.Value += 4)
				.Intercept(y => y.Value += 5));

			var instance = container.GetService<Wrapper<string>>();

			instance.Value.ShouldBe("12345");
		}

		class Parent { }

		class Child : Parent
		{
			public Parent Parent { get; set; }
		}

		class Wrapper<T>
		{
			public T Value { get; set; }
		}

		class StringWrapperInterceptor : Interceptor<Wrapper<string>>
		{
			public override Wrapper<string> Execute(Wrapper<string> instance, Maestro.Context context)
			{
				instance.Value += 3;
				return instance;
			}
		}

		[Fact]
		public void interceptors_should_not_be_executed_if_instance_is_cached()
		{
			var counter = 0;
			var container = new Container(x => x.Use<object>().Type<object>().Intercept(_ => counter++).Lifetime.Singleton());

			container.GetService<object>();
			counter.ShouldBe(1);
			container.GetService<object>();
			counter.ShouldBe(1);
		}
	}
}