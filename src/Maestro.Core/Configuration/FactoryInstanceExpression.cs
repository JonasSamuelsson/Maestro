using System;
using Maestro.Interceptors;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class FactoryInstanceExpression<T> : IFactoryInstanceExpression<T>
	{
		public FactoryInstanceExpression(Plugin plugin)
		{
			InstanceExpression = new InstanceExpression<T, IFactoryInstanceExpression<T>>(plugin, this);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> InstanceExpression { get; set; }

		public ILifetimeExpression<IFactoryInstanceExpression<T>> Lifetime => InstanceExpression.Lifetime;

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> Execute(Action<T> action)
		{
			return InstanceExpression.Execute(action);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> Execute(Action<T, IContext> action)
		{
			return InstanceExpression.Execute(action);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> Execute(Func<T, T> func)
		{
			return InstanceExpression.Execute(func);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> Execute(Func<T, IContext, T> func)
		{
			return InstanceExpression.Execute(func);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> Execute(IInterceptor interceptor)
		{
			return InstanceExpression.Execute(interceptor);
		}
	}
}