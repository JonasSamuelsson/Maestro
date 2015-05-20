using System;

namespace Maestro.Internals
{
	interface IPipelineFactory
	{
		bool TryGet(Type type, Context context, out IPipeline pipeline);
	}
}