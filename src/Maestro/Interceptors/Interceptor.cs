using System;

namespace Maestro.Interceptors
{
	internal abstract class Interceptor<T> : IInterceptor
	{
		public abstract T Execute(T instance, Context context);
		
		object IInterceptor.Execute(object instance, Context context)
		{
			return Execute((T)instance, context);
		}
	}
}