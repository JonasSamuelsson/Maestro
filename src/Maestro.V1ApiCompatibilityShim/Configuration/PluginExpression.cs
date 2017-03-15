using System;

namespace Maestro.Configuration
{
	internal class PluginExpression : IPluginExpression, IDefaultPluginExpression
	{
		private readonly IServiceExpression _serviceExpression;
		private readonly IServiceExpression _servicesExpression;

		public PluginExpression(IServiceExpression serviceExpression, IServiceExpression servicesExpression)
		{
			_serviceExpression = serviceExpression;
			_servicesExpression = servicesExpression;
		}

		public void Use(object instance)
		{
			_serviceExpression.Use.Instance(instance);
		}

		public IFactoryInstanceConfigurator<object> Use(Func<object> factory)
		{
			return _serviceExpression.Use.Factory(factory);
		}

		public IFactoryInstanceConfigurator<object> Use(Func<IContext, object> factory)
		{
			return _serviceExpression.Use.Factory(factory);
		}

		public ITypeInstanceConfigurator<object> Use(Type type)
		{
			return _serviceExpression.Use.Type(type);
		}

		public void TryUse(object instance)
		{
			_serviceExpression.TryUse.Instance(instance);
		}

		public IFactoryInstanceConfigurator<object> TryUse(Func<object> factory)
		{
			return _serviceExpression.TryUse.Factory(factory);
		}

		public IFactoryInstanceConfigurator<object> TryUse(Func<IContext, object> factory)
		{
			return _serviceExpression.TryUse.Factory(factory);
		}

		public ITypeInstanceConfigurator<object> TryUse(Type type)
		{
			return _serviceExpression.TryUse.Type(type);
		}

		public void Add(object instance)
		{
			_servicesExpression.Add.Instance(instance);
		}

		public IFactoryInstanceConfigurator<object> Add(Func<object> factory)
		{
			return _servicesExpression.Add.Factory(factory);
		}

		public IFactoryInstanceConfigurator<object> Add(Func<IContext, object> factory)
		{
			return _servicesExpression.Add.Factory(factory);
		}

		public ITypeInstanceConfigurator<object> Add(Type type)
		{
			return _servicesExpression.Add.Type(type);
		}
	}

	internal class PluginExpression<T> : IPluginExpression<T>, IDefaultPluginExpression<T>
	{
		private readonly IServiceExpression<T> _serviceExpression;
		private readonly IServiceExpression<T> _servicesExpression;

		public PluginExpression(IServiceExpression<T> serviceExpression, IServiceExpression<T> servicesExpression)
		{
			_serviceExpression = serviceExpression;
			_servicesExpression = servicesExpression;
		}

		public void Use(T instance)
		{
			_serviceExpression.Use.Instance(instance);
		}

		public IFactoryInstanceConfigurator<TInstance> Use<TInstance>(Func<TInstance> factory) where TInstance : T
		{
			return _serviceExpression.Use.Factory(factory);
		}

		public IFactoryInstanceConfigurator<TInstance> Use<TInstance>(Func<IContext, TInstance> factory) where TInstance : T
		{
			return _serviceExpression.Use.Factory(factory);
		}

		public ITypeInstanceConfigurator<TInstance> Use<TInstance>() where TInstance : T
		{
			return _serviceExpression.Use.Type<TInstance>();
		}

		public void TryUse(T instance)
		{
			_serviceExpression.TryUse.Instance(instance);
		}

		public IFactoryInstanceConfigurator<TInstance> TryUse<TInstance>(Func<TInstance> factory) where TInstance : T
		{
			return _serviceExpression.TryUse.Factory(factory);
		}

		public IFactoryInstanceConfigurator<TInstance> TryUse<TInstance>(Func<IContext, TInstance> factory) where TInstance : T
		{
			return _serviceExpression.TryUse.Factory(factory);
		}

		public ITypeInstanceConfigurator<TInstance> TryUse<TInstance>() where TInstance : T
		{
			return _serviceExpression.TryUse.Type<TInstance>();
		}

		public void Add(T instance)
		{
			_servicesExpression.Add.Instance(instance);
		}

		public IFactoryInstanceConfigurator<TInstance> Add<TInstance>(Func<TInstance> factory) where TInstance : T
		{
			return _servicesExpression.Add.Factory(factory);
		}

		public IFactoryInstanceConfigurator<TInstance> Add<TInstance>(Func<IContext, TInstance> factory) where TInstance : T
		{
			return _servicesExpression.Add.Factory(factory);
		}

		public ITypeInstanceConfigurator<TInstance> Add<TInstance>() where TInstance : T
		{
			return _servicesExpression.Add.Type<TInstance>();
		}
	}
}