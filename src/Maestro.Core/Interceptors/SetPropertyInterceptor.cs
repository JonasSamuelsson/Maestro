using System;

namespace Maestro.Interceptors
{
	internal class SetPropertyInterceptor : Interceptor<object>
	{
		// todo : perf

		private readonly string _propertyName;
		private readonly string _serviceName;
		private readonly Func<IContext, Type, object> _factory;
		private Action<object, IContext> _worker;

		public SetPropertyInterceptor(string propertyName, string serviceName)
		{
			_propertyName = propertyName;
			_serviceName = serviceName;
			_factory = (ctx, type) => ctx.GetService(type, _serviceName);
			_worker = Initialize;
		}

		public SetPropertyInterceptor(string propertyName, Func<IContext, Type, object> factory)
		{
			_propertyName = propertyName;
			_factory = factory;
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
			_worker = (o, ctx) => setter(o, _factory(ctx, property.PropertyType));
			_worker.Invoke(instance, context);
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