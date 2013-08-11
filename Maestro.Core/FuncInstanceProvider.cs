using System;

namespace Maestro
{
	internal class FuncInstanceProvider : IProvider
	{
		private readonly Func<IContext, object> _func;

		public FuncInstanceProvider(Func<IContext, object> func)
		{
			_func = func;
		}

		public bool CanGet(IContext context)
		{
			return true;
		}

		public object Get(IContext context)
		{
			return _func(context);
		}
	}
}