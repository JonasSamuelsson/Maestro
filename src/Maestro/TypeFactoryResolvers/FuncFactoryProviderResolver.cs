using Maestro.FactoryProviders;
using Maestro.Internals;
using System;
using System.Linq;

namespace Maestro.TypeFactoryResolvers
{
	internal class FuncFactoryProviderResolver : IFactoryProviderResolver
	{
		public bool TryGet(Type type, string name, Context context, out IFactoryProvider factoryProvider)
		{
			factoryProvider = null;
			if (!type.IsConcreteClassClosing(typeof(Func<>))) return false;
			var serviceType = type.GetGenericArguments().Single();
			if (!context.CanGetService(serviceType, name)) return false;

			var wrapperType = typeof(Wrapper<>).MakeGenericType(serviceType);
			var ctx = context;
			var wrapper = Activator.CreateInstance(wrapperType, name, ctx.ScopedContainer, ctx.Kernel);
			var getFuncMethod = wrapperType.GetMethod("GetFunc", new Type[] { });
			var lambda = new Func<Context, object>(_ => getFuncMethod.Invoke(wrapper, null)); // todo perf
			factoryProvider = new LambdaFactoryProvider(lambda);
			return true;
		}

		class Wrapper<T>
		{
			private readonly string _name;
			private readonly Container _container;
			private readonly Kernel _kernel;

			public Wrapper(string name, Container container, Kernel kernel)
			{
				_name = name;
				_container = container;
				_kernel = kernel;
			}

			// ReSharper disable once UnusedMember.Local
			public Func<T> GetFunc()
			{
				return GetInstance;
			}

			private T GetInstance()
			{
				using (var context = new Context(_container, _kernel))
					return context.GetService<T>(_name);
			}
		}
	}
}