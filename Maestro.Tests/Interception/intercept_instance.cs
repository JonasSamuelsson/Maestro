using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using FluentAssertions;
using Xunit;
using IInterceptor = Maestro.Interceptors.IInterceptor;

namespace Maestro.Tests.Interception
{
	public class intercept_instance
	{
		[Fact]
		public void interceptors_should_be_executed_in_the_same_order_they_are_configured()
		{
			var list = new List<string>();

			new Container(x => x.For<object>().Use<object>()
				.InterceptWith(new Interceptor(() => list.Add("create1")))
				.InterceptWith(new Interceptor(() => list.Add("create2")))
				.InterceptWith(new Interceptor(() => list.Add("activate1")))
				.InterceptWith(new Interceptor(() => list.Add("activate2"))))
			.Get<object>();

			list.Should().ContainInOrder(new[] { "create1", "create2", "activate1", "activate2" });
		}

		[Fact]
		public void insterceptors_should_not_be_executed_if_instance_is_cached()
		{
			var interceptor = new Interceptor();

			new Container(x => x.For<object>().Use<object>()
				.InterceptWith(interceptor)
				.Lifetime.Singleton()).Get<object>();

			interceptor.ExecuteCount.Should().Be(1);
		}

		[Fact]
		public void interceptors_should_not_be_executed_if_instance_is_chached()
		{
			var interceptor = new Interceptor();

			var container = new Container(x => x.For<object>().Use<object>()
				.InterceptWith(interceptor)
				.Lifetime.Singleton());
			
			interceptor.ExecuteCount.Should().Be(0);
			container.Get<object>();
			interceptor.ExecuteCount.Should().Be(1);
			container.Get<object>();
			interceptor.ExecuteCount.Should().Be(1);
		}

		[Fact]
		public void dynamic_proxy_interception()
		{
			var interceptor = new DynamicProxyInterceptor();
			var container = new Container(x => x.For(typeof(ITarget))
			                                    .Use(() => (object)new Target())
															.InterceptWithProxy((o, pg) => pg.CreateInterfaceProxyWithTarget((ITarget)o, interceptor)));

			container.Get<ITarget>().ToString();

			interceptor.Executed.Should().BeTrue();
		}

		public interface ITarget
		{
			string ToString();
		}

		private class Target : ITarget { }

		public class DynamicProxyInterceptor : Castle.DynamicProxy.IInterceptor
		{
			public void Intercept(IInvocation invocation)
			{
				Executed = true;
				invocation.Proceed();
			}

			public bool Executed { get; private set; }
		}

		private class Interceptor : IInterceptor
		{
			private readonly Action _action;

			public Interceptor() : this(() => { }) { }
			public Interceptor(Action action)
			{
				_action = action;
			}

			public int ExecuteCount { get; private set; }

			public IInterceptor Clone()
			{
				throw new NotImplementedException();
			}

			public object Execute(object instance, IContext context)
			{
				_action();
				ExecuteCount++;
				return instance;
			}
		}
	}
}