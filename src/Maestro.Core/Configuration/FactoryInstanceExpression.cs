using System;
using System.Linq.Expressions;
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

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> Intercept(Action<T> action)
		{
			return InstanceExpression.Intercept(action);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> Intercept(Action<T, IContext> action)
		{
			return InstanceExpression.Intercept(action);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> Intercept(Func<T, T> func)
		{
			return InstanceExpression.Intercept(func);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> Intercept(Func<T, IContext, T> func)
		{
			return InstanceExpression.Intercept(func);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> Intercept(IInterceptor interceptor)
		{
			return InstanceExpression.Intercept(interceptor);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> SetProperty<TValue>(Expression<Func<T, TValue>> property)
		{
			return InstanceExpression.SetProperty(property);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> SetProperty(string property)
		{
			return InstanceExpression.SetProperty(property);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> SetProperty(string property, object value)
		{
			return InstanceExpression.SetProperty(property, value);
		}
	}
}