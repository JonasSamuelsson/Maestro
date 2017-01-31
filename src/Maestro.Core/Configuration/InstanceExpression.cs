using System;
using System.Linq.Expressions;
using Maestro.FactoryProviders;
using Maestro.Interceptors;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class InstanceExpression<TInstance, TParent> : IInstanceExpression<TInstance, TParent>
	{
		public InstanceExpression(ServiceDescriptor serviceDescriptor, TParent parent)
		{
			ServiceDescriptor = serviceDescriptor;
			Parent = parent;
		}

		internal ServiceDescriptor ServiceDescriptor { get; set; }
		internal TParent Parent { get; set; }

		public ILifetimeExpression<TParent> Lifetime
		{
			get { return new LifetimeExpression<TParent>(Parent, lifetime => ServiceDescriptor.Lifetime = lifetime); }
		}

		public IInstanceExpression<TInstance, TParent> Intercept(Action<TInstance> action)
		{
			return Intercept((instance, ctx) => action(instance));
		}

		public IInstanceExpression<TInstance, TParent> Intercept(Action<TInstance, IContext> action)
		{
			return Intercept(new ActionInterceptor<TInstance>(action));
		}

		public IInstanceExpression<TInstance, TParent> Intercept(Func<TInstance, TInstance> func)
		{
			return Intercept((instance, ctx) => func(instance));
		}

		public IInstanceExpression<TInstance, TParent> Intercept(Func<TInstance, IContext, TInstance> func)
		{
			return Intercept(new FuncInterceptor<TInstance>(func));
		}

		public IInstanceExpression<TInstance, TParent> Intercept(IInterceptor interceptor)
		{
			ServiceDescriptor.Interceptors.Add(interceptor);
			return this;
		}

		public IInstanceExpression<TInstance, TParent> SetProperty(string property)
		{
			return Intercept(new SetPropertyInterceptor(property, ServiceDescriptor.Name));
		}

		public IInstanceExpression<TInstance, TParent> SetProperty(string property, object value)
		{
			return SetProperty(property, (ctx, type) => value);
		}

		public IInstanceExpression<TInstance, TParent> SetProperty(string property, Func<object> factory)
		{
			return SetProperty(property, (ctx, type) => factory());
		}

		public IInstanceExpression<TInstance, TParent> SetProperty(string property, Func<IContext, object> factory)
		{
			return Intercept(new SetPropertyInterceptor(property, (ctx, type) => factory(ctx)));
		}

		public IInstanceExpression<TInstance, TParent> SetProperty(string property, Func<IContext, Type, object> factory)
		{
			return Intercept(new SetPropertyInterceptor(property, factory));
		}

		public IInstanceExpression<TInstance, TParent> SetProperty<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return SetProperty(GetPropertyName(property));
		}

		public IInstanceExpression<TInstance, TParent> SetProperty<TValue>(Expression<Func<TInstance, TValue>> property, TValue value)
		{
			return SetProperty(GetPropertyName(property), value);
		}

		public IInstanceExpression<TInstance, TParent> SetProperty<TValue>(Expression<Func<TInstance, TValue>> property, Func<TValue> factory)
		{
			return SetProperty(GetPropertyName(property), (ctx, type) => factory());
		}

		public IInstanceExpression<TInstance, TParent> SetProperty<TValue>(Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory)
		{
			return SetProperty(GetPropertyName(property), (ctx, type) => factory(ctx));
		}

		public IInstanceExpression<TInstance, TParent> TrySetProperty(string property)
		{
			return Intercept(new TrySetPropertyInterceptor(property, ServiceDescriptor.Name));
		}

		public IInstanceExpression<TInstance, TParent> TrySetProperty<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return TrySetProperty(GetPropertyName(property));
		}

		private static string GetPropertyName<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return ((MemberExpression)property.Body).Member.Name;
		}

		public TParent CtorArg(string argName, Func<IContext, Type, object> factory)
		{
			var typeFactoryProvider = (TypeFactoryProvider)ServiceDescriptor.FactoryProvider;
			var ctorArg = new TypeFactoryProvider.CtorArg { Name = argName, Factory = factory };
			typeFactoryProvider.CtorArgs.Add(ctorArg);
			return Parent;
		}

		public TParent CtorArg(Type argType, Func<IContext, Type, object> factory)
		{
			var typeFactoryProvider = (TypeFactoryProvider)ServiceDescriptor.FactoryProvider;
			var ctorArg = new TypeFactoryProvider.CtorArg { Type = argType, Factory = factory };
			typeFactoryProvider.CtorArgs.Add(ctorArg);
			return Parent;
		}
	}
}