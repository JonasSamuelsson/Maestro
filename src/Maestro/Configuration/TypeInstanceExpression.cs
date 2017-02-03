using System;
using System.Linq.Expressions;
using Maestro.Interceptors;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class TypeInstanceExpression<T> : ITypeInstanceExpression<T>
	{
		public TypeInstanceExpression(ServiceDescriptor serviceDescriptor)
		{
			ServiceDescriptor = serviceDescriptor;
			InstanceExpression = new InstanceExpression<T, ITypeInstanceExpression<T>>(serviceDescriptor, this);
		}

		internal ServiceDescriptor ServiceDescriptor { get; }
		public InstanceExpression<T, ITypeInstanceExpression<T>> InstanceExpression { get; }
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

		public IInstanceExpression<T, ITypeInstanceExpression<T>> SetProperty(string property)
		{
			return InstanceExpression.SetProperty(property);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> SetProperty(string property, object value)
		{
			return InstanceExpression.SetProperty(property, value);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> SetProperty(string property, Func<object> factory)
		{
			return InstanceExpression.SetProperty(property, factory);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> SetProperty(string property, Func<IContext, object> factory)
		{
			return InstanceExpression.SetProperty(property, factory);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> SetProperty(string property, Func<IContext, Type, object> factory)
		{
			return InstanceExpression.SetProperty(property, factory);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> SetProperty<TValue>(Expression<Func<T, TValue>> property)
		{
			return InstanceExpression.SetProperty(property);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> SetProperty<TValue>(Expression<Func<T, TValue>> property, TValue value)
		{
			return InstanceExpression.SetProperty(property, value);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> SetProperty<TValue>(Expression<Func<T, TValue>> property, Func<TValue> factory)
		{
			return InstanceExpression.SetProperty(property, factory);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> SetProperty<TValue>(Expression<Func<T, TValue>> property, Func<IContext, TValue> factory)
		{
			return InstanceExpression.SetProperty(property, factory);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> TrySetProperty(string property)
		{
			return InstanceExpression.TrySetProperty(property);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> TrySetProperty<TValue>(Expression<Func<T, TValue>> property)
		{
			return InstanceExpression.TrySetProperty(property);
		}

		public ITypeInstanceExpression<T> CtorArg(string argName, object value)
		{
			return InstanceExpression.CtorArg(argName, (ctx, type) => value);
		}

		public ITypeInstanceExpression<T> CtorArg(string argName, Func<object> factory)
		{
			return InstanceExpression.CtorArg(argName, (ctx, type) => factory());
		}

		public ITypeInstanceExpression<T> CtorArg(string argName, Func<IContext, object> factory)
		{
			return InstanceExpression.CtorArg(argName, (ctx, type) => factory(ctx));
		}

		public ITypeInstanceExpression<T> CtorArg(string argName, Func<IContext, Type, object> factory)
		{
			return InstanceExpression.CtorArg(argName, factory);
		}

		public ITypeInstanceExpression<T> CtorArg(Type argType, object value)
		{
			return InstanceExpression.CtorArg(argType, (ctx, type) => value);
		}

		public ITypeInstanceExpression<T> CtorArg(Type argType, Func<object> factory)
		{
			return InstanceExpression.CtorArg(argType, (ctx, type) => factory());
		}

		public ITypeInstanceExpression<T> CtorArg(Type argType, Func<IContext, object> factory)
		{
			return InstanceExpression.CtorArg(argType, (ctx, type) => factory(ctx));
		}

		public ITypeInstanceExpression<T> CtorArg<TValue>(TValue value)
		{
			return InstanceExpression.CtorArg(typeof(TValue), (ctx, type) => value);
		}

		public ITypeInstanceExpression<T> CtorArg<TValue>(Func<TValue> factory)
		{
			return InstanceExpression.CtorArg(typeof(TValue), (ctx, type) => factory());
		}

		public ITypeInstanceExpression<T> CtorArg<TValue>(Func<IContext, TValue> factory)
		{
			return InstanceExpression.CtorArg(typeof(TValue), (ctx, type) => factory(ctx));
		}
	}
}