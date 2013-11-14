using System;

namespace Maestro.Factories
{
	internal class LambdaInstanceFactory : IInstanceFactory
	{
		private readonly Func<IContext, object> _lambda;

		public LambdaInstanceFactory(Func<IContext, object> lambda)
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

		public IInstanceFactory MakeGenericInstanceFactory(Type[] types)
		{
			return this;
		}

		public IInstanceFactory Clone()
		{
			return this;
		}

		public override string ToString()
		{
			return "lambda instance";
		}
	}
}