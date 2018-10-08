using System.Collections.Generic;
using Castle.DynamicProxy;
using Maestro.Configuration;
using Shouldly;
using Xunit;

namespace Maestro.Castle.DynamicProxy.Tests
{
	public class DynamicProxyTests
	{
		[Fact]
		public void should_wrap_factory_instance_with_dynamic_proxy()
		{
			var interceptor = new Interceptor();

			var container = new Container(x =>
			{
				x.Add<IInterface>().Named("x").Factory(() => new Implementation())
					.Proxy((y, generator) => generator.CreateInterfaceProxyWithTarget<IInterface>(y, interceptor));
				x.Add<IInterface>().Named("y").Factory(() => new Implementation())
					.Proxy((y, ctx, generator) => generator.CreateInterfaceProxyWithTarget<IInterface>(y, interceptor));
			});

			foreach (var name in new[] { "x", "y" })
			{
				container.GetService<IInterface>(name).DoWork();
				interceptor.InvokedMethods.ShouldBe(new[] { "DoWork" });
				interceptor.InvokedMethods.Clear();
			}
		}

		[Fact]
		public void should_wrap_type_instance_with_dynamic_proxy()
		{
			var interceptor = new Interceptor();

			var container = new Container(x =>
			{
				x.Add<IInterface>().Named("x").Type<Implementation>()
					.Proxy((y, generator) => generator.CreateInterfaceProxyWithTarget<IInterface>(y, interceptor));
				x.Add<IInterface>().Named("y").Type<Implementation>()
					.Proxy((y, ctx, generator) => generator.CreateInterfaceProxyWithTarget<IInterface>(y, interceptor));
			});

			foreach (var name in new[] { "x", "y" })
			{
				container.GetService<IInterface>(name).DoWork();
				interceptor.InvokedMethods.ShouldBe(new[] { "DoWork" });
				interceptor.InvokedMethods.Clear();
			}
		}

		public interface IInterface
		{
			void DoWork();
		}

		class Implementation : IInterface
		{
			public void DoWork() { }
		}

		public class Interceptor : IInterceptor
		{
			public List<string> InvokedMethods { get; } = new List<string>();

			public void Intercept(IInvocation invocation)
			{
				InvokedMethods.Add(invocation.Method.Name);
				invocation.Proceed();
			}
		}
	}
}