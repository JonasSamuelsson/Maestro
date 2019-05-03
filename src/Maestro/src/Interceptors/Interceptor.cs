using System;

namespace Maestro.Interceptors
{
	internal abstract class Interceptor
	{
		internal abstract object Execute(object instance, Context context);
		internal abstract Interceptor MakeGeneric(Type[] genericArguments);
	}

	internal abstract class Interceptor<T> : Interceptor
	{
		internal override object Execute(object instance, Context context)
		{
			return Execute((T)instance, context);
		}

		public abstract T Execute(T instance, Context context);
	}
}