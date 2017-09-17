﻿using Maestro.Internals;
using System;
using Maestro.Configuration;

namespace Maestro.Interceptors
{
	internal class TrySetPropertyInterceptor : Interceptor<object>
	{
		private static readonly PropertyProvider PropertyProvider = new PropertyProvider();

		private readonly string _propertyName;
		private readonly string _serviceName;
		private readonly PropertyNotFoundAction _propertyNotFoundAction;
		private Action<object, IContext> _worker;

		public TrySetPropertyInterceptor(string propertyName, PropertyNotFoundAction propertyNotFoundAction, string serviceName)
		{
			_propertyName = propertyName;
			_serviceName = serviceName;
			_propertyNotFoundAction = propertyNotFoundAction;
			_worker = InitializeWorker;
		}

		public override object Execute(object instance, IContext context)
		{
			_worker.Invoke(instance, context);
			return instance;
		}

		private void InitializeWorker(object instance, IContext context)
		{
			var type = instance.GetType();
			var property = PropertyProvider.GetProperty(type, _propertyName);

			if (property == null)
			{
				if (_propertyNotFoundAction == PropertyNotFoundAction.Throw)
					throw new InvalidOperationException($"Could not find property '{type.FullName}.{_propertyName}'.");

				_worker = (o, ctx) => { };
				return;
			}

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
			return new TrySetPropertyInterceptor(_propertyName, _propertyNotFoundAction, _serviceName);
		}

		public override string ToString()
		{
			return $"try set property {_propertyName}";
		}
	}
}