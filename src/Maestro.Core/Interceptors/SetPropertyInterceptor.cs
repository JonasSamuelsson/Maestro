using System;

namespace Maestro.Interceptors
{
	internal class SetPropertyInterceptor : Interceptor<object>
	{
		// todo : perf

		private readonly string _propertyName;
		private readonly string _serviceName;
		private Func<IContext, Type, object> _factory;
		private Func<IContext, object> _factoryAdapter;
		private SetPropertyAction _setPropertyAction;

		public SetPropertyInterceptor(string propertyName, string serviceName)
		{
			_propertyName = propertyName;
			_serviceName = serviceName;
		}

		public SetPropertyInterceptor(string propertyName, Func<IContext, Type, object> factory)
		{
			_propertyName = propertyName;
			_factory = factory;
		}

		public override object Execute(object instance, IContext context)
		{
			if (_setPropertyAction == null)
			{
				var property = instance.GetType().GetProperty(_propertyName);

				if (_factory == null)
				{
					_factory = GetValueFactory(property.PropertyType, _serviceName);
				}

				_factoryAdapter = ctx => _factory.Invoke(ctx, property.PropertyType);
				_setPropertyAction = SetPropertyActionFactory.Get(property);
			}

			var value = _factoryAdapter.Invoke(context);
			_setPropertyAction.Invoke(instance, value);
			return instance;
		}

		private static Func<IContext, Type, object> GetValueFactory(Type propertyType, string serviceName)
		{
			return (context, type) => context.GetService(propertyType, serviceName);
		}

		public override IInterceptor MakeGeneric(Type[] genericArguments)
		{
			return _serviceName != null
				? new SetPropertyInterceptor(_propertyName, _serviceName)
				: new SetPropertyInterceptor(_propertyName, _factory);
		}

		public override string ToString()
		{
			return $"set property {_propertyName}";
		}
	}
}