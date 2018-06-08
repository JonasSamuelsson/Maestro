using System;

namespace Maestro.Interceptors
{
	internal class FuncInterceptor<T> : Interceptor<T>
	{
		private readonly Func<T, Context, T> _func;

		public FuncInterceptor(Func<T, Context, T> func)
		{
			_func = func;
		}

		public override T Execute(T instance, Context context)
		{
			return _func(instance, context);
		}

		public override string ToString()
		{
			return "func interceptor";
		}
	}
}