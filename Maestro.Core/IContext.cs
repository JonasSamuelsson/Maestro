using System;

namespace Maestro
{
	public interface IContext
	{
		long RequestId { get; }
		string Name { get; }
		int ConfigId { get; }

		bool CanGet(Type type);
		object Get(Type type);
	}
}