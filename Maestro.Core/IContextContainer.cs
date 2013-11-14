using System;
using System.Collections.Generic;

namespace Maestro
{
	internal interface IContextContainer
	{
		Guid Id { get; }
		bool CanGet(Type type, IContext context);
		object Get(Type type, IContext context);
		IEnumerable<object> GetAll(Type type, IContext context);

		event Action<Guid> Disposed;
	}
}