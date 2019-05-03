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

		internal override Interceptor MakeGeneric(Type[] genericArguments)
		{
			return this;
		}

		public override string ToString()
		{
			return "custom func interceptor";
		}
	}
}