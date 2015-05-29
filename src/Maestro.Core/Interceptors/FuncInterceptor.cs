using System;

namespace Maestro.Interceptors
{
	public class FuncInterceptor<T> : Interceptor<T>
	{
		private readonly Func<T, IContext, T> _func;

		public FuncInterceptor(Func<T, IContext, T> func)
		{
			_func = func;
		}

		public override T Execute(T instance, IContext context)
		{
			return _func(instance, context);
		}

		public override string ToString()
		{
			return "func interceptor";
		}
	}
}