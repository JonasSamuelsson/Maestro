using System;

namespace Maestro
{
	internal class Context : IContext, IDisposable
	{
		private readonly IDependencyContainer _container;

		public Context(int configId, long requestId, string name, IDependencyContainer container)
		{
			ConfigId = configId;
			RequestId = requestId;
			Name = name;
			_container = container;

			TypeStack = new TypeStack();
		}

		public int ConfigId { get; private set; }
		public long RequestId { get; private set; }
		public string Name { get; internal set; }
		public ITypeStack TypeStack { get; private set; }

		public bool CanGet(Type type)
		{
			return _container.CanGet(type, this);
		}

		public object Get(Type type)
		{
			return _container.Get(type, this);
		}

		public event Action Disposed;

		public void Dispose()
		{
			var action = Disposed;
			if (action == null) return;
			action();
		}
	}
}