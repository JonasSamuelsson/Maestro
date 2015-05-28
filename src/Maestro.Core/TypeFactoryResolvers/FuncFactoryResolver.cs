using System;
using System.Linq;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.TypeFactoryResolvers
{
	class FuncFactoryResolver : ITypeFactoryResolver
	{
		public bool TryGet(Type type, Context context, out Pipeline pipeline)
		{
			pipeline = null;
			if (!type.IsConcreteClassClosing(typeof(Func<>))) return false;
			var typeArgument = type.GetGenericArguments().Single();
			if (!context.Kernel.CanGetDependency(typeArgument, context)) return false;

			var wrapperType = typeof(Wrapper<>).MakeGenericType(typeArgument);
			var wrapper = Activator.CreateInstance(wrapperType, context.Name, context.Kernel);
			var getFuncMethod = wrapperType.GetMethod("GetFunc");
			var lambda = new Func<IContext, object>(_ => getFuncMethod.Invoke(wrapper, null));
			pipeline = new Pipeline
			{
				Plugin = new Plugin
				{
					Type = type,
					Name = context.Name,
					FactoryProvider = new LambdaFactoryProvider(lambda)
				}
			};
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
				return (T)_kernel.Get(typeof(T), _name);
			}
		}
	}
}