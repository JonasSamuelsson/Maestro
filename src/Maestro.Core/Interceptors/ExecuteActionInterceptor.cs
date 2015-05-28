using System;

namespace Maestro.Interceptors
{
	internal class ExecuteActionInterceptor<T> : InterceptorBase<T, T>
	{
		private readonly Action<T, IContext> _action;

		public ExecuteActionInterceptor(Action<T, IContext> action)
		{
			_action = action;
		}

		public override IInterceptor Clone()
		{
			return this;
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