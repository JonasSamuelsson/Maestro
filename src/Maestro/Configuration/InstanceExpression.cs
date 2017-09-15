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
			return Intercept(new SetPropertyInterceptor(property, ServiceDescriptor.Name, throwIfPropertyDoesntExist: true));
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
			return SetProperty(property, (ctx, type) => factory(ctx));
		}

		public TParent SetProperty(string property, Func<IContext, Type, object> factory)
		{
			return Intercept(new SetPropertyInterceptor(property, factory, throwIfPropertyDoesntExist: true));
		}

		public TParent SetPropertyIfExists(string property)
		{
			return Intercept(new SetPropertyInterceptor(property, ServiceDescriptor.Name, throwIfPropertyDoesntExist: false));
		}

		public TParent SetPropertyIfExists(string property, object value)
		{
			return SetPropertyIfExists(property, (ctx, type) => value);
		}

		public TParent SetPropertyIfExists(string property, Func<object> factory)
		{
			return SetPropertyIfExists(property, (ctx, type) => factory());
		}

		public TParent SetPropertyIfExists(string property, Func<IContext, object> factory)
		{
			return SetPropertyIfExists(property, (ctx, type) => factory(ctx));
		}

		public TParent SetPropertyIfExists(string property, Func<IContext, Type, object> factory)
		{
			return Intercept(new SetPropertyInterceptor(property, factory, throwIfPropertyDoesntExist: false));
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
			return Intercept(new TrySetPropertyInterceptor(property, ServiceDescriptor.Name, throwIfPropertyDoesntExist: true));
		}

		public TParent TrySetProperty<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return TrySetProperty(GetPropertyName(property));
		}

		public TParent TrySetPropertyIfExists(string property)
		{
			return Intercept(new TrySetPropertyInterceptor(property, ServiceDescriptor.Name, throwIfPropertyDoesntExist: false));
		}
		
		private static string GetPropertyName<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return ((MemberExpression)property.Body).Member.Name;
		}
	}
}