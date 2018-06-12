using System;

namespace Maestro.Diagnostics
{
	internal class PipelineService
	{
		internal int? Id { get; set; }
		internal string Provider { get; set; }
		internal Type InstanceType { get; set; }
	}
}