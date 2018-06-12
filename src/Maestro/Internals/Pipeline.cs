using System.Collections.Generic;
using Maestro.Diagnostics;

namespace Maestro.Internals
{
	internal abstract class Pipeline
	{
		internal abstract object Execute(Context context);
		internal abstract void Populate(List<PipelineService> services);
	}
}