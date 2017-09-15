﻿using Maestro.Internals;
using System;

namespace Maestro.Interceptors
{
	internal class SetPropertyInterceptor : Interceptor<object>
	{
		private static readonly PropertyProvider PropertyProvider = new PropertyProvider();

		private readonly string _propertyName;
		private readonly string _serviceName;
		private readonly bool _throwIfPropertyDoesntExist;
		private readonly Func<IContext, Type, object> _factory;
		private Action<object, IContext> _worker;

		public SetPropertyInterceptor(string propertyName, string serviceName, bool throwIfPropertyDoesntExist)
		{
			_propertyName = propertyName;
			_serviceName = serviceName;
			_throwIfPropertyDoesntExist = throwIfPropertyDoesntExist;
			_factory = (ctx, type) => ctx.GetService(type, _serviceName);
			_worker = InitializeWorker;
		}

		public SetPropertyInterceptor(string propertyName, Func<IContext, Type, object> factory, bool throwIfPropertyDoesntExist)
		{
			_propertyName = propertyName;
			_factory = factory;
			_throwIfPropertyDoesntExist = throwIfPropertyDoesntExist;
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
				if (_throwIfPropertyDoesntExist)
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
				? new SetPropertyInterceptor(_propertyName, _serviceName, _throwIfPropertyDoesntExist)
				: new SetPropertyInterceptor(_propertyName, _factory, _throwIfPropertyDoesntExist);
		}

		public override string ToString()
		{
			return $"set property {_propertyName}";
		}
	}
}