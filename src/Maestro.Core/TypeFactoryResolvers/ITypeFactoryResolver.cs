using System;
using Maestro.Internals;

namespace Maestro.TypeFactoryResolvers
{
	interface ITypeFactoryResolver
	{
		bool TryGet(Type type, Context context, out Internals.IPipeline pipeline);
	}
}