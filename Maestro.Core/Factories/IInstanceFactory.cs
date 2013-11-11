using System;

namespace Maestro.Factories
{
	internal interface IInstanceFactory
	{
		bool CanGet(IContext context);
		object Get(IContext context);
		IInstanceFactory MakeGenericInstanceFactory(Type[] types);
	}
}