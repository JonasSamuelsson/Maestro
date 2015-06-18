using System;

namespace Maestro.Interceptors
{
	public interface IInterceptor
	{
		object Execute(object instance, IContext context);
		IInterceptor MakeGeneric(Type[] genericArguments);
	}
}