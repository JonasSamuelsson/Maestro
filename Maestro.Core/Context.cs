using System;
using System.Collections.Generic;

namespace Maestro
{
	internal class Context : IContext
	{
		private readonly IDependencyContainer _container;
		private readonly List<Type> _canGetHistory;
		private readonly List<Type> _getHistory;

		public Context(long requestId, string name, IDependencyContainer container)
		{
			Name = name;
			RequestId = requestId;
			_container = container;
			_canGetHistory = new List<Type>();
			_getHistory = new List<Type>();
		}

		public string Name { get; internal set; }
		public long RequestId { get; private set; }

		public bool CanGet(Type type)
		{
			if (_canGetHistory.Contains(type)) return false;
			_canGetHistory.Add(type);
			var result = _container.CanGet(type, this);
			_canGetHistory.Remove(type);
			return result;
		}

		public object Get(Type type)
		{
			if (_getHistory.Contains(type)) throw new ActivationException(string.Format("Cyclic dependency {0}-{1}.", Name, type));
			_getHistory.Add(type);
			var instance = _container.Get(type, this);
			_getHistory.Remove(type);
			return instance;
		}
	}
}