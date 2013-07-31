using System;

namespace Maestro
{
	internal interface IDependencyContainer
	{
		bool CanGet(Type type, IContext context);
		object Get(Type type, IContext context);
	}
}