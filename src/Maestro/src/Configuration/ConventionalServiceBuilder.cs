using Maestro.Interceptors;
using System;
using System.Linq.Expressions;

namespace Maestro.Configuration
{
	internal class ConventionalServiceBuilder<T> : IConventionalServiceRegistrator<T>, IConventionalServiceBuilder<T>
	{
		private readonly IContainerBuilder _containerBuilder;
		private readonly Type _baseType;
		private readonly Type _type;
		private Func<IServiceBuilder> _serviceBuilderFactory;
		private ITypeInstanceBuilder<T> _typeInstanceBuilder;

		public ConventionalServiceBuilder(IContainerBuilder containerBuilder, Type baseType, Type type)
		{
			_containerBuilder = containerBuilder;
			_baseType = baseType;
			_type = type;

			_serviceBuilderFactory = () => _containerBuilder.Add(baseType);
		}

		public IConventionalServiceBuilder<T> Add()
		{
			return Add(ServiceType.BaseType);
		}

		public IConventionalServiceBuilder<T> Add(ServiceType serviceType)
		{
			var type = serviceType == ServiceType.BaseType ? _baseType : _type;
			_serviceBuilderFactory = () => _containerBuilder.Add(type);
			return this;
		}

		public IConventionalServiceBuilder<T> AddOrThrow()
		{
			return AddOrThrow(ServiceType.BaseType);
		}

		public IConventionalServiceBuilder<T> AddOrThrow(ServiceType serviceType)
		{
			var type = serviceType == ServiceType.BaseType ? _baseType : _type;
			_serviceBuilderFactory = () => _containerBuilder.AddOrThrow(type);
			return this;
		}

		public IConventionalServiceBuilder<T> TryAdd()
		{
			return TryAdd(ServiceType.BaseType);
		}

		public IConventionalServiceBuilder<T> TryAdd(ServiceType serviceType)
		{
			var type = serviceType == ServiceType.BaseType ? _baseType : _type;
			_serviceBuilderFactory = () => _containerBuilder.TryAdd(type);
			return this;
		}

		public ITypeInstanceBuilder<T> Named(string name)
		{
			_typeInstanceBuilder = _serviceBuilderFactory.Invoke().Named(name).Type(_type).As<T>();
			return this;
		}

		private ITypeInstanceBuilder<T> Builder() => _typeInstanceBuilder ?? (_typeInstanceBuilder = _serviceBuilderFactory.Invoke().Type(_type).As<T>());

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