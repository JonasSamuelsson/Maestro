using System;

namespace Maestro.Interceptors
{
	internal class TrySetPropertyInterceptor : Interceptor<object>
	{
		// todo : perf

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

		{
		}

		public override string ToString()
		{
			return $"try set property {_propertyName}";
		}
	}
}
