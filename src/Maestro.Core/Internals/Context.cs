using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
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

		bool IContext.CanGet<T>()
		{
			return ((IContext)this).CanGet(typeof(T));
		}

		bool IContext.CanGet(Type type)
		{
			AssertNotDisposed();
			return Kernel.CanGetDependency(type, this);
		}

		T IContext.Get<T>()
		{
			return (T)((IContext)this).Get(typeof(T));
		}

		object IContext.Get(Type type)
		{
			object instance;
			if (((IContext)this).TryGet(type, out instance))
				return instance;

			throw new NotImplementedException("foobar");
		}

		bool IContext.TryGet<T>(out T instance)
		{
			object o;
			var result = ((IContext)this).TryGet(typeof(T), out o);
			instance = (T)o;
			return result;
		}

		bool IContext.TryGet(Type type, out object instance)
		{
			AssertNotDisposed();
			return Kernel.TryGet(type, this, out instance);
		}

		IEnumerable<T> IContext.GetAll<T>()
		{
			return ((IContext)this).GetAll(typeof(T)).Cast<T>().ToArray();
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
				throw new InvalidOperationException($"Cyclic dependency, '{type.FullName}'.");
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
			Disposed?.Invoke(this);
		}

		public event Action<Context> Disposed;
	}
}