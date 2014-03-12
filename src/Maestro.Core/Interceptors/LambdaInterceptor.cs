using System;

namespace Maestro.Interceptors
{
	internal class LambdaInterceptor<TIn, TOut> : InterceptorBase<TIn, TOut>
	{
		private readonly Func<TIn, IContext, TOut> _lambda;

		public LambdaInterceptor(Func<TIn, IContext, TOut> lambda)
		{
			_lambda = lambda;
		}

		public override IInterceptor Clone()
		{
			return this;
		}

		public override TOut Execute(TIn instance, IContext context)
		{
			return _lambda(instance, context);
		}

		public override string ToString()
		{
			return "lambda interceptor";
		}
	}
}