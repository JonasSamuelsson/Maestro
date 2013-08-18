using System;

namespace Maestro
{
	internal class LambdaInstanceProvider : IProvider
	{
		private readonly Func<IContext, object> _lambda;

		public LambdaInstanceProvider(Func<IContext, object> lambda)
		{
			_lambda = lambda;
		}

		public bool CanGet(IContext context)
		{
			return true;
		}

		public object Get(IContext context)
		{
			return _lambda(context);
		}

		public IProvider MakeGenericProvider(Type[] types)
		{
			throw new NotImplementedException();
		}
	}
}