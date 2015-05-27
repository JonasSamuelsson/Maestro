using System.Collections.Generic;

namespace Maestro.Internals
{
	interface IPipelineLookup
	{
		void Add(string key, Pipeline pipeline);
		void Add(string key, IEnumerable<Pipeline> pipelines);
		bool TryGet(string key, out Pipeline pipeline);
		bool TryGet(string key, out IEnumerable<Pipeline> pipelines);
	}
}