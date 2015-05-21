using System;
using System.Collections.Generic;

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
		private readonly Stack<Type> _stack = new Stack<Type>();
		private bool _disposed;

		public Context(string name, Kernel kernel)
		{
			Name = name;
			Kernel = kernel;
		}

		public string Name { get; }
		public Kernel Kernel { get; }

		bool IContext.TryGet(Type type, out object instance)
		{
			AssertNotDisposed();
			return Kernel.TryGet(type, this, out instance);
		}

		IEnumerable<object> IContext.GetAll(Type type)
		{
			AssertNotDisposed();
			return Kernel.GetAll(type, this);
		}

		public void PushStackFrame(Type type)
		{
			if (_stack.Contains(type))
			{
				throw new InvalidOperationException("Cyclic dependency");
			}

			_stack.Push(type);
		}

		public void PopStackFrame()
		{
			_stack.Pop();
		}

		private void AssertNotDisposed()
		{
			if (_disposed) throw new ObjectDisposedException("Context has been disposed.");
		}

		public void Dispose()
		{
			_disposed = true;
		}
	}
}