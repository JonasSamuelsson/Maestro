using System.Collections.Generic;

namespace Maestro.Internals
{
	interface IPipelineLookup
	{
		void Add(string key, IPipeline pipeline);
		void Add(string key, IEnumerable<IPipeline> pipelines);
		bool TryGet(string key, out IPipeline pipeline);
		bool TryGet(string key, out IEnumerable<IPipeline> pipelines);
	}
}