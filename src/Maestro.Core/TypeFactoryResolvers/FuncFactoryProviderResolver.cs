using System;
using System.Linq;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.TypeFactoryResolvers
{
	class FuncFactoryProviderResolver : IFactoryProviderResolver
	{
		public bool TryGet(Type type, string name, IContext context, out IFactoryProvider factoryProvider)
		{
			factoryProvider = null;
			if (!type.IsConcreteClassClosing(typeof(Func<>))) return false;
			var serviceType = type.GetGenericArguments().Single();
			if (!context.CanGetService(serviceType, name)) return false;

			var wrapperType = typeof(Wrapper<>).MakeGenericType(serviceType);
			var ctx = (Context)context;
			var wrapper = Activator.CreateInstance(wrapperType, name, ctx.Kernel);
			var getFuncMethod = wrapperType.GetMethod("GetFunc");
			var lambda = new Func<IContext, object>(_ => getFuncMethod.Invoke(wrapper, null)); // todo perf
			factoryProvider = new LambdaFactoryProvider(lambda);
			return true;
		}

		class Wrapper<T>
		{
			private readonly string _name;
			private readonly Kernel _kernel;

			public Wrapper(string name, Kernel kernel)
			{
				_name = name;
				_kernel = kernel;
			}

			public Func<T> GetFunc()
			{
				return GetInstance;
			}

			private T GetInstance()
			{
				using (var context = new Context(_name, _kernel))
					return context.GetService<T>(_name);
			}
		}
	}
}