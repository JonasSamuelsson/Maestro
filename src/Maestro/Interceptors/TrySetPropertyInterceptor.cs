using System;

namespace Maestro.Interceptors
{
	internal class TrySetPropertyInterceptor : Interceptor<object>
	{
		private readonly string _propertyName;
		private readonly string _serviceName;
		private Action<object, IContext> _worker;

		public TrySetPropertyInterceptor(string propertyName, string serviceName)
		{
			_propertyName = propertyName;
			_serviceName = serviceName;
			_worker = Initialize;
		}

		public override object Execute(object instance, IContext context)
		{
			_worker.Invoke(instance, context);
			return instance;
		}

		private void Initialize(object instance, IContext context)
		{
			var property = instance.GetType().GetProperty(_propertyName);
			var setter = PropertySetter.Create(property);

			_worker = (o, ctx) =>
			{
				object service;
				if (!ctx.TryGetService(property.PropertyType, _serviceName, out service)) return;
				setter(o, service);
			};

			_worker.Invoke(instance, context);
		}

		public IInterceptor MakeGeneric(Type[] genericArguments)
		{
			return new TrySetPropertyInterceptor(_propertyName, _serviceName);
		}

		public override string ToString()
		{
			return $"try set property {_propertyName}";
		}
	}
}