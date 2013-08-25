using System;

namespace Maestro
{
	public interface IContext
	{
		int ConfigId { get; }
		long RequestId { get; }
		string Name { get; }
		ITypeStack TypeStack { get; }

		bool CanGet(Type type);
		object Get(Type type);

		event Action Disposed;
	}
}