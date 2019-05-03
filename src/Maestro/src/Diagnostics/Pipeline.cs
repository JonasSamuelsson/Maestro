using System;
using System.Collections.Generic;

namespace Maestro.Diagnostics
{
	internal class Pipeline
	{
		internal Type Type { get; set; }
		internal string Name { get; set; }
		internal List<PipelineService> Services { get; set; } = new List<PipelineService>();
	}
}