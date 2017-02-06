using System;

namespace Maestro.Interceptors
{
	internal abstract class Interceptor<T> : IInterceptor
	{
		public abstract T Execute(T instance, IContext context);

		public virtual IInterceptor MakeGeneric(Type[] genericArguments)
		{
			// todo
			throw new NotImplementedException();
		}

		object IInterceptor.Execute(object instance, IContext context)
		{
			return Execute((T)instance, context);
		}
	}
}