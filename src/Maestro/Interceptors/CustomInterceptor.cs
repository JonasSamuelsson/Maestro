using System;

namespace Maestro.Interceptors
{
	internal class CustomInterceptor : Interceptor
	{
		private readonly IInterceptor _interceptor;

		public CustomInterceptor(IInterceptor interceptor)
		{
			_interceptor = interceptor;
		}

		internal override object Execute(object instance, Context context)
		{
			return _interceptor.Execute(instance, context);
		}

		internal override Interceptor MakeGeneric(Type[] genericArguments)
		{
			return this;
		}

		public override string ToString()
		{
			return $"custom interceptor: '{GetType().ToFormattedString()}'";
		}
	}
}