using System;

namespace Maestro.Interceptors
{
	 class ActionInterceptor<T> : Interceptor<T>
	{
		private readonly Action<T, IContext> _action;

		public ActionInterceptor(Action<T, IContext> action)
		{
			_action = action;
		}

		public override T Execute(T instance, IContext context)
		{
			_action(instance, context);
			return instance;
		}

		public override string ToString()
		{
			return "action interceptor";
		}
	}
}