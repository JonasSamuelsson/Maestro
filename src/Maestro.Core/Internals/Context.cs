using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	interface IContext
	{
		//bool CanGet(Type type);
		//object Get(Type type);
		bool TryGet(Type type, out object instance);

		//bool CanGet<T>();
		//T Get<T>();
		//bool TryGet<T>(out T instance);

		IEnumerable<object> GetAll(Type type);
		//IEnumerable<T> GetAll<T>();
	}

	class Context : IContext, IDisposable
	{
		private readonly string _name;
		private readonly Kernel _kernel;
		private readonly HashSet<Type> _processedTypes = new HashSet<Type>();
		private bool _disposed;

		public Context(string name, Kernel kernel)
		{
			_name = name;
			_kernel = kernel;
		}

		public bool CanGet(Type type)
		{
			try
			{
				AssertNotDisposed();
				Push(type);
				return _kernel.CanGet(type, _name, this);
			}
			finally
			{
				_processedTypes.Remove(type);
			}
		}

		public bool TryGet(Type type, out object instance)
		{
			try
			{
				AssertNotDisposed();
				Push(type);
				return _kernel.TryGet(type, _name, this, out instance);
			}
			finally
			{
				_processedTypes.Remove(type);
			}
		}

		public IEnumerable<object> GetAll(Type type)
		{
			try
			{
				AssertNotDisposed();
				Push(type);
				return _kernel.GetAll(type, this);
			}
			finally
			{
				_processedTypes.Remove(type);
			}
		}

		private void AssertNotDisposed()
		{
			if (_disposed) throw new ObjectDisposedException("Context has been disposed.");
		}

		private void Push(Type type)
		{
			if (_processedTypes.Add(type)) return;
			throw new InvalidOperationException("Cyclic dependency.");
		}

		public bool CanGetDependency(Type type)
		{
			return CanGet(type) || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
		}

		public bool TryGetDependency(Type type, out object instance)
		{
			if (TryGet(type, out instance)) return true;

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				instance = GetAll(type.GetGenericArguments().Single());
				return true;
			}

			return false;
		}

		public object GetDependency(Type type)
		{
			object instance;
			if (TryGetDependency(type, out instance)) return instance;
			throw new InvalidOperationException();
		}

		public void Dispose()
		{
			_disposed = true;
		}
	}
}