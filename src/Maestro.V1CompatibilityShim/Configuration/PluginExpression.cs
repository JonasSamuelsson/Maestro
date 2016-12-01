using System;

namespace Maestro.Configuration
{
	internal class PluginExpression : IPluginExpression, IDefaultPluginExpression
	{
		private readonly IServiceExpression _serviceExpression;
		private readonly IServicesExpression _servicesExpression;

		public PluginExpression(IServiceExpression serviceExpression, IServicesExpression servicesExpression)
		{
			_serviceExpression = serviceExpression;
			_servicesExpression = servicesExpression;
		}

		public void Use(object instance)
		{
			_serviceExpression.Use.Instance(instance);
		}

		public IFactoryInstanceExpression<object> Use(Func<object> factory)
		{
			return _serviceExpression.Use.Factory(factory);
		}

		public IFactoryInstanceExpression<object> Use(Func<IContext, object> factory)
		{
			return _serviceExpression.Use.Factory(factory);
		}

		public ITypeInstanceExpression<object> Use(Type type)
		{
			return _serviceExpression.Use.Type(type);
		}

		public void TryUse(object instance)
		{
			_serviceExpression.TryUse.Instance(instance);
		}

		public IFactoryInstanceExpression<object> TryUse(Func<object> factory)
		{
			return _serviceExpression.TryUse.Factory(factory);
		}

		public IFactoryInstanceExpression<object> TryUse(Func<IContext, object> factory)
		{
			return _serviceExpression.TryUse.Factory(factory);
		}

		public ITypeInstanceExpression<object> TryUse(Type type)
		{
			return _serviceExpression.TryUse.Type(type);
		}

		public void Add(object instance)
		{
			_servicesExpression.Add.Instance(instance);
		}

		public IFactoryInstanceExpression<object> Add(Func<object> factory)
		{
			return _servicesExpression.Add.Factory(factory);
		}

		public IFactoryInstanceExpression<object> Add(Func<IContext, object> factory)
		{
			return _servicesExpression.Add.Factory(factory);
		}

		public ITypeInstanceExpression<object> Add(Type type)
		{
			return _servicesExpression.Add.Type(type);
		}
	}

	internal class PluginExpression<T> : IPluginExpression<T>, IDefaultPluginExpression<T>
	{
		private readonly IServiceExpression<T> _serviceExpression;
		private readonly IServicesExpression<T> _servicesExpression;

		public PluginExpression(IServiceExpression<T> serviceExpression, IServicesExpression<T> servicesExpression)
		{
			_serviceExpression = serviceExpression;
			_servicesExpression = servicesExpression;
		}

		public void Use(T instance)
		{
			_serviceExpression.Use.Instance(instance);
		}

		public IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<TInstance> factory) where TInstance : T
		{
			return _serviceExpression.Use.Factory(factory);
		}

		public IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> factory) where TInstance : T
		{
			return _serviceExpression.Use.Factory(factory);
		}

		public ITypeInstanceExpression<TInstance> Use<TInstance>() where TInstance : T
		{
			return _serviceExpression.Use.Type<TInstance>();
		}

		public void TryUse(T instance)
		{
			_serviceExpression.TryUse.Instance(instance);
		}

		public IFactoryInstanceExpression<TInstance> TryUse<TInstance>(Func<TInstance> factory) where TInstance : T
		{
			return _serviceExpression.TryUse.Factory(factory);
		}

		public IFactoryInstanceExpression<TInstance> TryUse<TInstance>(Func<IContext, TInstance> factory) where TInstance : T
		{
			return _serviceExpression.TryUse.Factory(factory);
		}

		public ITypeInstanceExpression<TInstance> TryUse<TInstance>() where TInstance : T
		{
			return _serviceExpression.TryUse.Type<TInstance>();
		}

		public void Add(T instance)
		{
			_servicesExpression.Add.Instance(instance);
		}

		public IFactoryInstanceExpression<TInstance> Add<TInstance>(Func<TInstance> factory) where TInstance : T
		{
			return _servicesExpression.Add.Factory(factory);
		}

		public IFactoryInstanceExpression<TInstance> Add<TInstance>(Func<IContext, TInstance> factory) where TInstance : T
		{
			return _servicesExpression.Add.Factory(factory);
		}

		public ITypeInstanceExpression<TInstance> Add<TInstance>() where TInstance : T
		{
			return _servicesExpression.Add.Type<TInstance>();
		}
	}
}