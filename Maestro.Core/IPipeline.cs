using System;

namespace Maestro
{
	internal interface IPipeline
	{
		bool CanGet(IContext context);
		object Get(IContext context);
		IPipeline MakeGenericPipeline(Type[] types);
	}
}