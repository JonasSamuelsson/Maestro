﻿using Maestro.Interceptors;
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
				x.Service<Instance>().Use.Factory(() => i).Intercept(new InstanceInterceptor());
			});

			var instance = container.Get<Instance>();

			instance.ShouldNotBe(i);
			instance.InnerInstance.ShouldBe(i);
		}

		[Fact]
		public void should_execute_provided_func()
		{
			var i = new Instance();
			var container = new Container(x =>
			{
				x.Service<Instance>("1").Use.Factory(() => i).Intercept(instance => new Instance(instance));
				x.Service<Instance>("2").Use.Factory(() => i).Intercept((instance, ctx) => new Instance(instance));
			});

			var instance1 = container.Get<Instance>("1");
			var instance2 = container.Get<Instance>("2");

			instance1.ShouldNotBe(i);
			instance1.InnerInstance.ShouldBe(i);

			instance2.ShouldNotBe(i);
			instance2.InnerInstance.ShouldBe(i);
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
				x.Service<NumberWrapper>("1").Use.Type<NumberWrapper>().Intercept(instance => instance.Number = 1);
				x.Service<NumberWrapper>("2").Use.Type<NumberWrapper>().Intercept((instance, ctx) => instance.Number = 2);
			});

			container.Get<NumberWrapper>("1").Number.ShouldBe(1);
			container.Get<NumberWrapper>("2").Number.ShouldBe(2);
		}

		class NumberWrapper
		{
			public int Number { get; set; }
		}

		[Fact]
		public void interceptors_should_be_executed_in_the_same_order_as_they_are_configured()
		{
			var container = new Container(x => x.Service<TextWrapper>().Use.Type<TextWrapper>()
				.Intercept(y => y.Text += 1)
				.Intercept(y =>
				{
					y.Text += 2;
					return y;
				})
				.Intercept(new TextWrapperInterceptor())
				.Intercept(y =>
				{
					y.Text += 4;
					return y;
				})
				.Intercept(y => y.Text += 5));

			var instance = container.Get<TextWrapper>();

			instance.Text.ShouldBe("12345");
		}

		class TextWrapper
		{
			public string Text { get; set; }
		}

		class TextWrapperInterceptor : Interceptor<TextWrapper>
		{
			public override TextWrapper Execute(TextWrapper instance, IContext context)
			{
				instance.Text += 3;
				return instance;
			}
		}

		[Fact]
		public void interceptors_should_not_be_executed_if_instance_is_cached()
		{
			var counter = 0;
			var container = new Container(x => x.Service<object>().Use.Type<object>().Intercept(_ => counter++).Lifetime.Singleton());

			container.Get<object>();
			counter.ShouldBe(1);
			container.Get<object>();
			counter.ShouldBe(1);
		}

		[Fact]
		public void set_property_with_provided_value()
		{
			var container = new Container(x =>
													{
														x.Services<TextWrapper>().Add.Type<TextWrapper>().SetProperty("Text", "success");
														x.Services<TextWrapper>().Add.Type<TextWrapper>().SetProperty(y => y.Text, "success");
													});

			var instances = container.GetAll<TextWrapper>();

			instances.ShouldAllBe(x => x.Text == "success");
		}

		[Fact]
		public void set_property_using_provided_factory()
		{
			var container = new Container(x =>
													{
														x.Services<TextWrapper>().Add.Type<TextWrapper>().SetProperty("Text", () => "success");
														x.Services<TextWrapper>().Add.Type<TextWrapper>().SetProperty("Text", ctx => "success");
														x.Services<TextWrapper>().Add.Type<TextWrapper>().SetProperty(y => y.Text, () => "success");
														x.Services<TextWrapper>().Add.Type<TextWrapper>().SetProperty(y => y.Text, ctx => "success");
													});

			var instances = container.GetAll<TextWrapper>();

			instances.ShouldAllBe(x => x.Text == "success");
		}

		class Parent
		{
			public Child Child { get; set; }
		}

		class Child { }

		[Fact]
		public void should_auto_inject_selected_property()
		{
			var container = new Container(x =>
													{
														x.Service<Parent>("1").Use.Type<Parent>().SetProperty("Child");
														x.Service<Parent>("2").Use.Type<Parent>().SetProperty(y => y.Child);
													});

			var instance1 = container.Get<Parent>("1");
			var instance2 = container.Get<Parent>("2");

			instance1.Child.ShouldNotBe(null);
			instance2.Child.ShouldNotBe(null);
		}

		//[Fact]
		//public void set_property_with_value_from_provided_func()
		//{
		//	var dependency = new object();
		//	var container = new Container(x => x.Service<Foobar>().Use<Foobar>().Set(y => y.ResolvableDependency, () => dependency));

		//	var instance = container.Get<Foobar>();

		//	instance.ResolvableDependency.Should().Be(dependency);
		//}

		//[Fact]
		//public void set_property_with_resolvable_type_should_work()
		//{
		//	var container = new Container(x => x.Service<Foobar>().Use<Foobar>().Set("ResolvableDependency"));
		//	var instance = container.Get<Foobar>();

		//	instance.ResolvableDependency.Should().NotBeNull();
		//}

		//[Fact]
		//public void set_missing_property_should_throw()
		//{
		//	var container = new Container(x => x.Service<Foobar>().Use<Foobar>().Set("missing property"));

		//	container.Invoking(x => x.Get<Foobar>()).ShouldThrow<ActivationException>();
		//}

		//[Fact]
		//public void set_property_with_unresolvable_type_should_throw()
		//{
		//	var container = new Container(x => x.Service<Foobar>().Use<Foobar>().Set(y => y.UnresolvableDependency));

		//	container.Invoking(x => x.Get<Foobar>()).ShouldThrow<ActivationException>();
		//}

		//[Fact]
		//public void try_set_property_with_unresolvable_type_should_work()
		//{
		//	var container = new Container(x => x.Service<Foobar>().Use<Foobar>().TrySet(y => y.UnresolvableDependency));

		//	container.Invoking(x => x.Get<Foobar>()).ShouldNotThrow();
		//}

		//[Fact]
		//public void try_set_missing_property_should_throw()
		//{
		//	var container = new Container(x => x.Service<Foobar>().Use<Foobar>().TrySet("missing property"));

		//	container.Invoking(x => x.Get<Foobar>()).ShouldThrow<ActivationException>();
		//}

		//[Fact]
		//public void set_enumerable_property_should_work()
		//{
		//	var o = new object();
		//	var container = new Container(x =>
		//											{
		//												x.Service<object>().Use(o);
		//												x.Service<Foobar>().Use<Foobar>().Set(y => y.Enumerable);
		//											});

		//	var instance = container.Get<Foobar>();

		//	instance.Enumerable.Should().BeEquivalentTo(new[] { o });
		//}

		//[Fact]
		//public void set_array_property_should_work()
		//{
		//	var o = new object();
		//	var container = new Container(x =>
		//											{
		//												x.Service<object>().Use(o);
		//												x.Service<Foobar>().Use<Foobar>().Set(y => y.Array);
		//											});

		//	var instance = container.Get<Foobar>();

		//	instance.Array.Should().BeEquivalentTo(new[] { o });
		//}

		//private class Foobar
		//{
		//	public object ResolvableDependency { get; set; }
		//	public IDisposable UnresolvableDependency { get; set; }
		//	public IEnumerable<object> Enumerable { get; set; }
		//	public object[] Array { get; set; }
		//}

		//public class property_injection_using_expressions : property_injection { }

		//public class property_injection_using_reflection : property_injection, IDisposable
		//{
		//	public property_injection_using_reflection()
		//	{
		//		Reflector.AlwaysUseReflection = true;
		//	}

		//	public void Dispose()
		//	{
		//		Reflector.AlwaysUseReflection = false;
		//	}
		//}
	}
}