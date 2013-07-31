using System;

namespace Maestro
{
	internal class Context : IContext
	{
		private readonly IDependencyContainer _container;

		public Context(long requestId, string name, IDependencyContainer container)
		{
			_container = container;
			RequestId = requestId;
			Name = name;
		}

		public long RequestId { get; private set; }
		public string Name { get; private set; }

		public bool CanGet(Type type)
		{
			return _container.CanGet(type, this);
		}

		public object Get(Type type)
		{
			return _container.Get(type, this);
		}
	}
}