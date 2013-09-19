using System;

namespace Maestro.Interceptors
{
	internal class LambdaInterceptor<TInstance> : IInterceptor
	{
		private readonly Func<TInstance, IContext, TInstance> _func;

		public LambdaInterceptor(Func<TInstance, IContext, TInstance> func)
		{
			_func = func;
		}

		public IInterceptor Clone()
		{
			return this;
		}

		public object Execute(object instance, IContext context)
		{
			return _func((TInstance)instance, context);
		}

		public override string ToString()
		{
			return "lambda interceptor";
		}
	}
}