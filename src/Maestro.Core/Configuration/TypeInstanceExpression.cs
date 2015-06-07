using System;
using System.Linq.Expressions;
using Maestro.FactoryProviders;
using Maestro.Interceptors;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class TypeInstanceExpression<T> : ITypeInstanceExpression<T>
	{
		public TypeInstanceExpression(Plugin plugin)
		{
			Plugin = plugin;
			InstanceExpression = new InstanceExpression<T, ITypeInstanceExpression<T>>(plugin, this);
		}

		internal Plugin Plugin { get; }
		public IInstanceExpression<T, ITypeInstanceExpression<T>> InstanceExpression { get; }

		public ILifetimeExpression<ITypeInstanceExpression<T>> Lifetime => InstanceExpression.Lifetime;

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Intercept(Action<T> action)
		{
			return InstanceExpression.Intercept(action);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Intercept(Action<T, IContext> action)
		{
			return InstanceExpression.Intercept(action);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Intercept(Func<T, T> func)
		{
			return InstanceExpression.Intercept(func);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Intercept(Func<T, IContext, T> func)
		{
			return InstanceExpression.Intercept(func);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Intercept(IInterceptor interceptor)
		{
			return InstanceExpression.Intercept(interceptor);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> SetProperty<TValue>(Expression<Func<T, TValue>> property)
		{
			return InstanceExpression.SetProperty(property);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> SetProperty(string property)
		{
			return InstanceExpression.SetProperty(property);
		}

		public ITypeInstanceExpression<T> ConstructorDependency<TDependency>(TDependency dependency)
		{
			return ConstructorDependency(typeof(TDependency), dependency);
		}

		public ITypeInstanceExpression<T> ConstructorDependency(Type type, object dependency)
		{
			((TypeFactoryProvider)Plugin.FactoryProvider).Dependencies.Add(type, dependency);
			return this;
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> SetProperty(string property, object value)
		{
			return InstanceExpression.SetProperty(property, value);
		}
	}
}