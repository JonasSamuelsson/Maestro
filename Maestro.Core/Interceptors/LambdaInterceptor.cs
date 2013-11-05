using System;

namespace Maestro.Interceptors
{
	internal class LambdaInterceptor<TIn, TOut> : IInterceptor
	{
		private readonly Func<TIn, IContext, TOut> _func;

		public LambdaInterceptor(Func<TIn, IContext, TOut> func)
		{
			_func = func;
		}

		public IInterceptor Clone()
		{
			return this;
		}

		public object Execute(object instance, IContext context)
		{
			return _func((TIn)instance, context);
		}

		public override string ToString()
		{
			return "lambda interceptor";
		}
	}
}