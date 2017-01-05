using System;

namespace Maestro.Lifetimes
{
	public interface ILifetime
	{
		object Execute(IContext context, Func<IContext, object> factory);
		ILifetime MakeGeneric(Type[] genericArguments);
	}
}