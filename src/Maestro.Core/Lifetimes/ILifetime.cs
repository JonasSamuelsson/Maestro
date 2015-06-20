using System;

namespace Maestro.Lifetimes
{
	public interface ILifetime
	{
		object Execute(INextStep nextStep);
		ILifetime MakeGeneric(Type[] genericArguments);
	}
}