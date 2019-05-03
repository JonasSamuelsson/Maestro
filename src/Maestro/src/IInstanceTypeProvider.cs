using System;

namespace Maestro
{
	public interface IInstanceTypeProvider
	{
		bool TryGetInstanceType(Type serviceType, Maestro.Context context, out Type instanceType);
	}
}