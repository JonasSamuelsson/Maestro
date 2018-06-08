using System;

namespace Maestro.Interceptors
{
	 class ActionInterceptor<T> : Interceptor<T>
	{
		private readonly Action<T, Context> _action;

		public ActionInterceptor(Action<T, Context> action)
		{
			_action = action;
		}

		public override T Execute(T instance, Context context)
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