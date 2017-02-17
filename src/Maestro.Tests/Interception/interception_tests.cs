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
				x.For<Instance>().Use.Factory(() => i).Intercept(new InstanceInterceptor());
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
			public override Instance Execute(Instance instance, IContext context)
			{
				return new Instance(instance);
			}
		}

		[Fact]
		public void should_execute_provided_action()
		{
			var container = new Container(x =>
			{
				x.For<Wrapper<int>>("1").Use.Type<Wrapper<int>>().Intercept(instance => instance.Value = 1);
				x.For<Wrapper<int>>("2").Use.Type<Wrapper<int>>().Intercept((instance, ctx) => instance.Value = 2);
			});

			container.GetService<Wrapper<int>>("1").Value.ShouldBe(1);
			container.GetService<Wrapper<int>>("2").Value.ShouldBe(2);
		}

		[Fact]
		public void interceptors_should_be_executed_in_the_same_order_as_they_are_configured()
		{
			var container = new Container(x => x.For<Wrapper<string>>().Use.Type<Wrapper<string>>()
				.Intercept(y => y.Value += 1)
				.Intercept(y => y.Value += 2)
				.Intercept(new StringWrapperInterceptor())
				.Intercept(y => y.Value += 4)
				.Intercept(y => y.Value += 5));

			var instance = container.GetService<Wrapper<string>>();

			instance.Value.ShouldBe("12345");
		}

		class Wrapper<T>
		{
			public T Value { get; set; }
		}

		class StringWrapperInterceptor : Interceptor<Wrapper<string>>
		{
			public override Wrapper<string> Execute(Wrapper<string> instance, IContext context)
			{
				instance.Value += 3;
				return instance;
			}
		}

		[Fact]
		public void interceptors_should_not_be_executed_if_instance_is_cached()
		{
			var counter = 0;
			var container = new Container(x => x.For<object>().Use.Type<object>().Intercept(_ => counter++).Lifetime.Singleton());

			container.GetService<object>();
			counter.ShouldBe(1);
			container.GetService<object>();
			counter.ShouldBe(1);
		}

		[Fact]
		public void set_property_using_auto_injection()
		{
			var continer = new Container(x =>
			{
				x.For<string>().Use.Instance("success-1");
				x.For<Wrapper<string>>().Use.Self().SetProperty(y => y.Value);
				x.For<string>("x").Use.Instance("success-2");
				x.For<Wrapper<string>>("x").Use.Self().SetProperty(y => y.Value);
				x.For<string>("y").Use.Instance("success-3");
				x.For(typeof(Wrapper<>), "y").Use.Self().SetProperty("Value");
			});

			continer.GetService<Wrapper<string>>().Value.ShouldBe("success-1");
			continer.GetService<Wrapper<string>>("x").Value.ShouldBe("success-2");
			continer.GetService<Wrapper<string>>("y").Value.ShouldBe("success-3");
		}

		[Fact]
		public void set_property_using_provided_value()
		{
			var container = new Container(x =>
													{
														x.For<Wrapper<string>>().Add.Self().SetProperty("Value", "success");
														x.For<Wrapper<string>>().Add.Self().SetProperty(y => y.Value, "success");
													});

			container.GetServices<Wrapper<string>>().ForEach(x => x.Value.ShouldBe("success"));
		}

		[Fact]
		public void set_property_using_provided_factory()
		{
			var container = new Container(x =>
													{
														x.For<string>().Use.Instance("success");
														x.For<Wrapper<string>>().Add.Self().SetProperty("Value", () => "success");
														x.For<Wrapper<string>>().Add.Self().SetProperty("Value", ctx => ctx.GetService<string>());
														x.For<Wrapper<string>>().Add.Self().SetProperty("Value", (ctx, type) => ctx.GetService(type));
														x.For(typeof(Wrapper<>)).Use.Self().SetProperty("Value", (ctx, type) => ctx.GetService(type));
														x.For<Wrapper<string>>().Add.Self().SetProperty(y => y.Value, () => "success");
														x.For<Wrapper<string>>().Add.Self().SetProperty(y => y.Value, ctx => ctx.GetService<string>());
													});

			container.GetServices<Wrapper<string>>().ForEach(x => x.Value.ShouldBe("success"));
		}

		[Fact]
		public void try_set_property()
		{
			var container = new Container(x =>
			{
				x.For<string>().Use.Instance("success");
				x.For<Wrapper<string>>().Add.Self().TrySetProperty("Value");
				x.For<Wrapper<string>>().Add.Self().TrySetProperty(y => y.Value);
				x.For(typeof(Wrapper<>)).Add.Self().TrySetProperty("Value");
			});

			container.GetServices<Wrapper<string>>().ForEach(x => x.Value.ShouldBe("success"));
		}
	}
}