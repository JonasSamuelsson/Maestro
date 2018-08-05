using Maestro.Interceptors;
using System;
using System.Linq.Expressions;

namespace Maestro.Configuration
{
	internal class ConventionalServiceBuilder<T> : IConventionalServiceRegistrator<T>, IConventionalServiceBuilder<T>
	{
		private readonly ContainerBuilder _containerBuilder;
		private readonly Type _serviceType;
		private readonly Type _instanceType;
		private Func<IServiceBuilder> _serviceBuilderFactory;
		private ITypeInstanceBuilder<T> _typeInstanceBuilder;

		public ConventionalServiceBuilder(ContainerBuilder containerBuilder, Type serviceType, Type instanceType)
		{
			_containerBuilder = containerBuilder;
			_serviceType = serviceType;
			_instanceType = instanceType;

			_serviceBuilderFactory = () => _containerBuilder.Add(serviceType);
		}

		public IConventionalServiceBuilder<T> Add()
		{
			return this;
		}

		public IConventionalServiceBuilder<T> AddOrThrow()
		{
			_serviceBuilderFactory = () => _containerBuilder.AddOrThrow(_serviceType);
			return this;
		}

		public IConventionalServiceBuilder<T> TryAdd()
		{
			_serviceBuilderFactory = () => _containerBuilder.TryAdd(_serviceType);
			return this;
		}

		public ITypeInstanceBuilder<T> Named(string name)
		{
			_typeInstanceBuilder = _serviceBuilderFactory.Invoke().Named(name).Type(_instanceType).As<T>();
			return this;
		}

		private ITypeInstanceBuilder<T> Builder() => _typeInstanceBuilder ?? (_typeInstanceBuilder = _serviceBuilderFactory.Invoke().Type(_instanceType).As<T>());

		public ITypeInstanceBuilder<T> Transient() => Builder().Transient();

		public ITypeInstanceBuilder<T> Scoped() => Builder().Scoped();

		public ITypeInstanceBuilder<T> Singleton() => Builder().Singleton();

		public ITypeInstanceBuilder<T> Intercept(Action<T> interceptor) => Builder().Intercept(interceptor);

		public ITypeInstanceBuilder<T> Intercept(Action<T, Context> interceptor) => Builder().Intercept(interceptor);

		public ITypeInstanceBuilder<T> Intercept(Func<T, T> interceptor) => Builder().Intercept(interceptor);

		public ITypeInstanceBuilder<T> Intercept(Func<T, Context, T> interceptor) => Builder().Intercept(interceptor);

		public ITypeInstanceBuilder<T> Intercept(IInterceptor interceptor) => Builder().Intercept(interceptor);

		public ITypeInstanceBuilder<T> SetProperty(string property, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw) => Builder().SetProperty(property, propertyNotFoundAction);

		public ITypeInstanceBuilder<T> SetProperty(string property, object value, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw) => Builder().SetProperty(property, value, propertyNotFoundAction);

		public ITypeInstanceBuilder<T> SetProperty(string property, Func<object> factory, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw) => Builder().SetProperty(property, factory, propertyNotFoundAction);

		public ITypeInstanceBuilder<T> SetProperty(string property, Func<Context, object> factory, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw) => Builder().SetProperty(property, factory, propertyNotFoundAction);

		public ITypeInstanceBuilder<T> SetProperty(string property, Func<Context, Type, object> factory, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw) => Builder().SetProperty(property, factory, propertyNotFoundAction);

		public ITypeInstanceBuilder<T> SetProperty<TValue>(Expression<Func<T, TValue>> property) => Builder().SetProperty(property);

		public ITypeInstanceBuilder<T> SetProperty<TValue>(Expression<Func<T, TValue>> property, TValue value) => Builder().SetProperty(property, value);

		public ITypeInstanceBuilder<T> SetProperty<TValue>(Expression<Func<T, TValue>> property, Func<TValue> factory) => Builder().SetProperty(property, factory);

		public ITypeInstanceBuilder<T> SetProperty<TValue>(Expression<Func<T, TValue>> property, Func<Context, TValue> factory) => Builder().SetProperty(property, factory);

		public ITypeInstanceBuilder<T> TrySetProperty(string property, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw) => Builder().TrySetProperty(property, propertyNotFoundAction);

		public ITypeInstanceBuilder<T> TrySetProperty<TValue>(Expression<Func<T, TValue>> property) => Builder().TrySetProperty(property);

		public ITypeInstanceBuilder<T> CtorArg(Type argType, object value) => Builder().CtorArg(argType, value);

		public ITypeInstanceBuilder<T> CtorArg(Type argType, Func<object> factory) => Builder().CtorArg(argType, factory);

		public ITypeInstanceBuilder<T> CtorArg(Type argType, Func<Context, object> factory) => Builder().CtorArg(argType, factory);

		public ITypeInstanceBuilder<T> CtorArg<TValue>(TValue value) => Builder().CtorArg(value);

		public ITypeInstanceBuilder<T> CtorArg<TValue>(Func<TValue> factory) => Builder().CtorArg(factory);

		public ITypeInstanceBuilder<T> CtorArg<TValue>(Func<Context, TValue> factory) => Builder().CtorArg(factory);

		public ITypeInstanceBuilder<TInstance> As<TInstance>() => Builder().As<TInstance>();

		public void Execute(Action<IConventionalServiceRegistrator<T>> action)
		{
			action(this);
			Builder();
		}
	}
}