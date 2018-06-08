using Maestro.Interceptors;
using Maestro.Internals;
using System;
using System.Linq.Expressions;

namespace Maestro.Configuration
{
	internal abstract class InstanceExpression<TInstance, TParent> : IInstanceExpression<TInstance, TParent>
	{
		protected InstanceExpression(ServiceDescriptor serviceDescriptor)
		{
			ServiceDescriptor = serviceDescriptor;
		}

		internal abstract TParent Parent { get; }
		internal ServiceDescriptor ServiceDescriptor { get; }

		public ILifetimeSelector<TParent> Lifetime
		{
			get { return new LifetimeSelector<TParent>(Parent, factory => ServiceDescriptor.Lifetime = factory()); }
		}

		public TParent Intercept(Action<TInstance> interceptor)
		{
			return Intercept((instance, ctx) => interceptor(instance));
		}

		public TParent Intercept(Action<TInstance, Context> interceptor)
		{
			return Intercept(new ActionInterceptor<TInstance>(interceptor));
		}

		public TParent Intercept(Func<TInstance, TInstance> interceptor)
		{
			return Intercept((instance, ctx) => interceptor(instance));
		}

		public TParent Intercept(Func<TInstance, Context, TInstance> interceptor)
		{
			return Intercept(new FuncInterceptor<TInstance>(interceptor));
		}

		public TParent Intercept(IInterceptor interceptor)
		{
			ServiceDescriptor.Interceptors.Add(interceptor);
			return Parent;
		}

		public TParent SetProperty(string property, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw)
		{
			return Intercept(new SetPropertyInterceptor(property, propertyNotFoundAction, ServiceDescriptor.Name));
		}

		public TParent SetProperty(string property, object value, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw)
		{
			return SetProperty(property, (ctx, type) => value, propertyNotFoundAction);
		}

		public TParent SetProperty(string property, Func<object> factory, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw)
		{
			return SetProperty(property, (ctx, type) => factory(), propertyNotFoundAction);
		}

		public TParent SetProperty(string property, Func<Context, object> factory, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw)
		{
			return SetProperty(property, (ctx, type) => factory(ctx), propertyNotFoundAction);
		}

		public TParent SetProperty(string property, Func<Context, Type, object> factory, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw)
		{
			return Intercept(new SetPropertyInterceptor(property, propertyNotFoundAction, factory));
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

		public TParent SetProperty<TValue>(Expression<Func<TInstance, TValue>> property, Func<Context, TValue> factory)
		{
			return SetProperty(GetPropertyName(property), (ctx, type) => factory(ctx));
		}

		public TParent TrySetProperty(string property, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw)
		{
			return Intercept(new TrySetPropertyInterceptor(property, propertyNotFoundAction, ServiceDescriptor.Name));
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