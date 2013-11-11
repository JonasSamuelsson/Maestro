using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Utils;

namespace Maestro
{
	internal class Context : IContext, IDisposable
	{
		private readonly IDependencyResolver _dependencyResolver;

		public Context(int configId, long requestId, string name, IDependencyResolver dependencyResolver)
		{
			ConfigId = configId;
			RequestId = requestId;
			Name = name;
			_dependencyResolver = dependencyResolver;

			TypeStack = new TypeStack();
		}

		public int ConfigId { get; private set; }
		public long RequestId { get; private set; }
		public string Name { get; internal set; }
		public ITypeStack TypeStack { get; private set; }

		public bool CanGet(Type type)
		{
			return _dependencyResolver.CanGet(type, this);
		}

		public bool CanGet<T>()
		{
			return CanGet(typeof(T));
		}

		public object Get(Type type)
		{
			return _dependencyResolver.Get(type, this);
		}

		public T Get<T>()
		{
			return (T)Get(typeof(T));
		}

		public IEnumerable<object> GetAll(Type type)
		{
			return _dependencyResolver.GetAll(type, this);
		}

		public IEnumerable<T> GetAll<T>()
		{
			return GetAll(typeof(T)).Cast<T>().ToList();
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