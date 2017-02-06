using System;
using System.Linq.Expressions;
using Maestro.Interceptors;
using Maestro.Internals;

namespace Maestro.Configuration
{
	internal abstract class InstanceExpression
	{
		protected InstanceExpression(ServiceDescriptor serviceDescriptor)
		{
			ServiceDescriptor = serviceDescriptor;
		}

		internal ServiceDescriptor ServiceDescriptor { get; }
	}
	internal abstract class InstanceExpression<TInstance, TParent> : InstanceExpression, IInstanceExpression<TInstance, TParent>
	{
		protected InstanceExpression(ServiceDescriptor serviceDescriptor) : base(serviceDescriptor)
		{
		}

		internal abstract TParent Parent { get; }

		public ILifetimeExpression<TParent> Lifetime
		{
			get { return new LifetimeExpression<TParent>(Parent, lifetime => ServiceDescriptor.Lifetime = lifetime); }
		}

		public TParent Intercept(Action<TInstance> interceptor)
		{
			return Intercept((instance, ctx) => interceptor(instance));
		}

		public TParent Intercept(Action<TInstance, IContext> interceptor)
		{
			return Intercept(new ActionInterceptor<TInstance>(interceptor));
		}

		public TParent Intercept(IInterceptor interceptor)
		{
			ServiceDescriptor.Interceptors.Add(interceptor);
			return Parent;
		}

		public TParent SetProperty(string property)
		{
			return Intercept(new SetPropertyInterceptor(property, ServiceDescriptor.Name));
		}

		public TParent SetProperty(string property, object value)
		{
			return SetProperty(property, (ctx, type) => value);
		}

		public TParent SetProperty(string property, Func<object> factory)
		{
			return SetProperty(property, (ctx, type) => factory());
		}

		public TParent SetProperty(string property, Func<IContext, object> factory)
		{
			return Intercept(new SetPropertyInterceptor(property, (ctx, type) => factory(ctx)));
		}

		public TParent SetProperty(string property, Func<IContext, Type, object> factory)
		{
			return Intercept(new SetPropertyInterceptor(property, factory));
		}

		public TParent SetProperty<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return SetProperty(GetPropertyName(property));
		}

		public TParent SetProperty<TValue>(Expression<Func<TInstance, TValue>> property, TValue value)
		{
			return SetProperty(GetPropertyName(property), value);
		}

		public TParent SetProperty<TValue>(Expression<Func<TInstance, TValue>> property, Func<TValue> factory)
		{
			return SetProperty(GetPropertyName(property), (ctx, type) => factory());
		}

		public TParent SetProperty<TValue>(Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory)
		{
			return SetProperty(GetPropertyName(property), (ctx, type) => factory(ctx));
		}

		public TParent TrySetProperty(string property)
		{
			return Intercept(new TrySetPropertyInterceptor(property, ServiceDescriptor.Name));
		}

		public TParent TrySetProperty<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return TrySetProperty(GetPropertyName(property));
		}

		private static string GetPropertyName<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return ((MemberExpression)property.Body).Member.Name;
		}
	}
}