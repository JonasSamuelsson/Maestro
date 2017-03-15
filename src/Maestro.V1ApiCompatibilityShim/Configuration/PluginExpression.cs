using System;

namespace Maestro.Configuration
{
	internal class PluginExpression : IPluginExpression, IDefaultPluginExpression
	{
		private readonly IServiceConfigurator _serviceConfigurator;
		private readonly IServiceConfigurator _servicesConfigurator;

		public PluginExpression(IServiceConfigurator serviceConfigurator, IServiceConfigurator servicesConfigurator)
		{
			_serviceConfigurator = serviceConfigurator;
			_servicesConfigurator = servicesConfigurator;
		}

		public void Use(object instance)
		{
			_serviceConfigurator.Use.Instance(instance);
		}

		public IFactoryInstanceConfigurator<object> Use(Func<object> factory)
		{
			return _serviceConfigurator.Use.Factory(factory);
		}

		public IFactoryInstanceConfigurator<object> Use(Func<IContext, object> factory)
		{
			return _serviceConfigurator.Use.Factory(factory);
		}

		public ITypeInstanceConfigurator<object> Use(Type type)
		{
			return _serviceConfigurator.Use.Type(type);
		}

		public void TryUse(object instance)
		{
			_serviceConfigurator.TryUse.Instance(instance);
		}

		public IFactoryInstanceConfigurator<object> TryUse(Func<object> factory)
		{
			return _serviceConfigurator.TryUse.Factory(factory);
		}

		public IFactoryInstanceConfigurator<object> TryUse(Func<IContext, object> factory)
		{
			return _serviceConfigurator.TryUse.Factory(factory);
		}

		public ITypeInstanceConfigurator<object> TryUse(Type type)
		{
			return _serviceConfigurator.TryUse.Type(type);
		}

		public void Add(object instance)
		{
			_servicesConfigurator.Add.Instance(instance);
		}

		public IFactoryInstanceConfigurator<object> Add(Func<object> factory)
		{
			return _servicesConfigurator.Add.Factory(factory);
		}

		public IFactoryInstanceConfigurator<object> Add(Func<IContext, object> factory)
		{
			return _servicesConfigurator.Add.Factory(factory);
		}

		public ITypeInstanceConfigurator<object> Add(Type type)
		{
			return _servicesConfigurator.Add.Type(type);
		}
	}

	internal class PluginExpression<T> : IPluginExpression<T>, IDefaultPluginExpression<T>
	{
		private readonly IServiceConfigurator<T> _serviceConfigurator;
		private readonly IServiceConfigurator<T> _servicesConfigurator;

		public PluginExpression(IServiceConfigurator<T> serviceConfigurator, IServiceConfigurator<T> servicesConfigurator)
		{
			_serviceConfigurator = serviceConfigurator;
			_servicesConfigurator = servicesConfigurator;
		}

		public void Use(T instance)
		{
			_serviceConfigurator.Use.Instance(instance);
		}

		public IFactoryInstanceConfigurator<TInstance> Use<TInstance>(Func<TInstance> factory) where TInstance : T
		{
			return _serviceConfigurator.Use.Factory(factory);
		}

		public IFactoryInstanceConfigurator<TInstance> Use<TInstance>(Func<IContext, TInstance> factory) where TInstance : T
		{
			return _serviceConfigurator.Use.Factory(factory);
		}

		public ITypeInstanceConfigurator<TInstance> Use<TInstance>() where TInstance : T
		{
			return _serviceConfigurator.Use.Type<TInstance>();
		}

		public void TryUse(T instance)
		{
			_serviceConfigurator.TryUse.Instance(instance);
		}

		public IFactoryInstanceConfigurator<TInstance> TryUse<TInstance>(Func<TInstance> factory) where TInstance : T
		{
			return _serviceConfigurator.TryUse.Factory(factory);
		}

		public IFactoryInstanceConfigurator<TInstance> TryUse<TInstance>(Func<IContext, TInstance> factory) where TInstance : T
		{
			return _serviceConfigurator.TryUse.Factory(factory);
		}

		public ITypeInstanceConfigurator<TInstance> TryUse<TInstance>() where TInstance : T
		{
			return _serviceConfigurator.TryUse.Type<TInstance>();
		}

		public void Add(T instance)
		{
			_servicesConfigurator.Add.Instance(instance);
		}

		public IFactoryInstanceConfigurator<TInstance> Add<TInstance>(Func<TInstance> factory) where TInstance : T
		{
			return _servicesConfigurator.Add.Factory(factory);
		}

		public IFactoryInstanceConfigurator<TInstance> Add<TInstance>(Func<IContext, TInstance> factory) where TInstance : T
		{
			return _servicesConfigurator.Add.Factory(factory);
		}

		public ITypeInstanceConfigurator<TInstance> Add<TInstance>() where TInstance : T
		{
			return _servicesConfigurator.Add.Type<TInstance>();
		}
	}
}