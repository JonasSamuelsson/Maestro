using System.Collections.Generic;

namespace Maestro
{
	internal interface IPlugin
	{
		void Add(string name, IPipelineEngine pipelineEngine);
		IPipelineEngine Get(string name);
		bool TryGet(string name, out IPipelineEngine pipelineEngine);
		IEnumerable<string> GetNames();
	}
}