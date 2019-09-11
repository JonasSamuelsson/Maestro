using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class InstantiationException : Exception
	{
		public InstantiationException(string message, IEnumerable<string> traceFrameInfos)
			: base(message)
		{
			TraceFrameInfos = traceFrameInfos.ToList();
		}

		public IEnumerable<string> TraceFrameInfos { get; }
	}
}