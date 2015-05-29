using Maestro.Interceptors;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Interception
{
	public class interceptor_tests
	{
		[Fact]
		public void should_execute_provided_interceptor()
		{
			var i = new Instance();
			var container = new Container(x =>
			{
				x.For<Instance>().Use(() => i).Execute(new InstanceInterceptor());
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
				x.For<Instance>("1").Use(() => i).Execute(instance => new Instance(instance));
				x.For<Instance>("2").Use(() => i).Execute((instance, ctx) => new Instance(instance));
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
				x.For<NumberWrapper>("1").Use<NumberWrapper>().Execute(instance => instance.Number = 1);
				x.For<NumberWrapper>("2").Use<NumberWrapper>().Execute((instance, ctx) => instance.Number = 2);
			});

			container.Get<NumberWrapper>("1").Number.ShouldBe(1);
			container.Get<NumberWrapper>("2").Number.ShouldBe(2);
		}

		class NumberWrapper
		{
			public int Number { get; set; }
		}

		//[Fact]
		//public void set_property_with_provided_value()
		//{
		//	var dependency = new object();
		//	var container = new Container(x => x.For<Foobar>().Use<Foobar>().Set(y => y.ResolvableDependency, dependency));

		//	var instance = container.Get<Foobar>();

		//	instance.ResolvableDependency.Should().Be(dependency);
		//}

		//[Fact]
		//public void set_property_with_value_from_provided_func()
		//{
		//	var dependency = new object();
		//	var container = new Container(x => x.For<Foobar>().Use<Foobar>().Set(y => y.ResolvableDependency, () => dependency));

		//	var instance = container.Get<Foobar>();

		//	instance.ResolvableDependency.Should().Be(dependency);
		//}

		//[Fact]
		//public void set_property_with_resolvable_type_should_work()
		//{
		//	var container = new Container(x => x.For<Foobar>().Use<Foobar>().Set("ResolvableDependency"));
		//	var instance = container.Get<Foobar>();

		//	instance.ResolvableDependency.Should().NotBeNull();
		//}

		//[Fact]
		//public void set_missing_property_should_throw()
		//{
		//	var container = new Container(x => x.For<Foobar>().Use<Foobar>().Set("missing property"));

		//	container.Invoking(x => x.Get<Foobar>()).ShouldThrow<ActivationException>();
		//}

		//[Fact]
		//public void set_property_with_unresolvable_type_should_throw()
		//{
		//	var container = new Container(x => x.For<Foobar>().Use<Foobar>().Set(y => y.UnresolvableDependency));

		//	container.Invoking(x => x.Get<Foobar>()).ShouldThrow<ActivationException>();
		//}

		//[Fact]
		//public void try_set_property_with_unresolvable_type_should_work()
		//{
		//	var container = new Container(x => x.For<Foobar>().Use<Foobar>().TrySet(y => y.UnresolvableDependency));

		//	container.Invoking(x => x.Get<Foobar>()).ShouldNotThrow();
		//}

		//[Fact]
		//public void try_set_missing_property_should_throw()
		//{
		//	var container = new Container(x => x.For<Foobar>().Use<Foobar>().TrySet("missing property"));

		//	container.Invoking(x => x.Get<Foobar>()).ShouldThrow<ActivationException>();
		//}

		//[Fact]
		//public void set_enumerable_property_should_work()
		//{
		//	var o = new object();
		//	var container = new Container(x =>
		//											{
		//												x.For<object>().Use(o);
		//												x.For<Foobar>().Use<Foobar>().Set(y => y.Enumerable);
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
		//												x.For<object>().Use(o);
		//												x.For<Foobar>().Use<Foobar>().Set(y => y.Array);
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