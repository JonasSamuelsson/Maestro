﻿using Maestro.Configuration;
using Maestro.Internals;
using System;

namespace Maestro.Interceptors
{
	internal class SetPropertyInterceptor : Interceptor<object>
	{
		private static readonly PropertyProvider PropertyProvider = new PropertyProvider();

		private readonly string _propertyName;
		private readonly PropertyNotFoundAction _propertyNotFoundAction;
		private readonly string _serviceName;
		private readonly Func<IContext, Type, object> _factory;
		private Action<object, IContext> _worker;

		public SetPropertyInterceptor(string propertyName, PropertyNotFoundAction propertyNotFoundAction, string serviceName)
		{
			_propertyName = propertyName;
			_propertyNotFoundAction = propertyNotFoundAction;
			_serviceName = serviceName;
			_factory = (ctx, type) => ctx.GetService(type, _serviceName);
			_worker = InitializeWorker;
		}

		public SetPropertyInterceptor(string propertyName, PropertyNotFoundAction propertyNotFoundAction, Func<IContext, Type, object> factory)
		{
			_propertyName = propertyName;
			_propertyNotFoundAction = propertyNotFoundAction;
			_factory = factory;
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
			_worker = (o, ctx) => setter(o, _factory(ctx, property.PropertyType));
			_worker.Invoke(instance, context);
		}

		public IInterceptor MakeGeneric(Type[] genericArguments)
		{
			return _serviceName != null
				? new SetPropertyInterceptor(_propertyName, _propertyNotFoundAction, _serviceName)
				: new SetPropertyInterceptor(_propertyName, _propertyNotFoundAction, _factory);
		}

		public override string ToString()
		{
			return $"set property {_propertyName}";
		}
	}
}