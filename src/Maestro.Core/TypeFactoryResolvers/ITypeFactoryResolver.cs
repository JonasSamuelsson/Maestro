using System;

namespace Maestro.Internals
{
	interface ITypeFactoryResolver
	{
		bool TryGet(Type type, Context context, out IPipeline pipeline);
	}
}