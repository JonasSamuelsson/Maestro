using Maestro.Configuration;
using Maestro.Internals;
using System;

namespace Maestro.Interceptors
{
	internal class TrySetPropertyInterceptor : Interceptor
	{
		private static readonly PropertyProvider PropertyProvider = new PropertyProvider();

		private readonly string _propertyName;
		private readonly string _serviceName;
		private readonly PropertyNotFoundAction _propertyNotFoundAction;
		private Action<object, Context> _worker;

		public TrySetPropertyInterceptor(string propertyName, PropertyNotFoundAction propertyNotFoundAction, string serviceName)
		{
			_propertyName = propertyName;
			_serviceName = serviceName;
			_propertyNotFoundAction = propertyNotFoundAction;
			_worker = InitializeWorker;
		}

		internal override object Execute(object instance, Context context)
		{
			_worker.Invoke(instance, context);
			return instance;
		}

		private void InitializeWorker(object instance, Context context)
		{
			var type = instance.GetType();
			var property = PropertyProvider.GetProperty(type, _propertyName);

			if (property == null)
			{
				if (_propertyNotFoundAction == PropertyNotFoundAction.Throw)
					throw new InvalidOperationException($"Could not find property '{type.ToFormattedString()}.{_propertyName}'.");

				_worker = (o, ctx) => { };
				return;
			}

			var setter = PropertySetter.Create(property);

			_worker = (o, ctx) =>
			{
				if (!ctx.TryGetService(property.PropertyType, _serviceName, out var service)) return;
				setter(o, service);
			};

			_worker.Invoke(instance, context);
		}

		internal override Interceptor MakeGeneric(Type[] genericArguments)
		{
			return new TrySetPropertyInterceptor(_propertyName, _propertyNotFoundAction, _serviceName);
		}

		public override string ToString()
		{
			return $"try set property {_propertyName}";
		}
	}
}