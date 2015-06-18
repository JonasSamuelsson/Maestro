using System;

namespace Maestro.Interceptors
{
	public abstract class Interceptor<T> : IInterceptor
	{
		public abstract T Execute(T instance, IContext context);

		public virtual IInterceptor MakeGeneric(Type[] genericArguments)
		{
			throw new NotImplementedException();
		}

		object IInterceptor.Execute(object instance, IContext context)
		{
			return Execute((T)instance, context);
		}
	}
}