using System;
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

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Execute(Action<T> action)
		{
			return InstanceExpression.Execute(action);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Execute(Action<T, IContext> action)
		{
			return InstanceExpression.Execute(action);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Execute(Func<T, T> func)
		{
			return InstanceExpression.Execute(func);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Execute(Func<T, IContext, T> func)
		{
			return InstanceExpression.Execute(func);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Execute(IInterceptor interceptor)
		{
			return InstanceExpression.Execute(interceptor);
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
	}
}