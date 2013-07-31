using System;

namespace Maestro
{
	internal interface IContext
	{
		long RequestId { get; }
		string Name { get; }

		bool CanGet(Type type);
		object Get(Type type);
	}
}