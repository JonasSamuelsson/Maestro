using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	class Context : IContext, IDisposable
	{
		private readonly Stack<Type> _processedTypes = new Stack<Type>();
		private bool _disposed;

		public Context(string name, Kernel kernel)
		{
			Name = name ?? ServiceDescriptorLookup.DefaultName;
			Kernel = kernel;
		}

		public string Name { get; }
		public Kernel Kernel { get; }

		public bool CanGetService(Type type)
		{
			var removeStackFrame = false;

			try
			{
				AssertNotDisposed();
				AddStackFrame(type);
				removeStackFrame = true;

				return Kernel.CanGetService(type, this);
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception);
			}
			finally
			{
				if (removeStackFrame) RemoveStackFrame();
			}
		}

		public bool CanGetService<T>()
		{
			try
			{
				return CanGetService(typeof(T));
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception);
			}
		}

		public object GetService(Type type)
		{
			try
			{
				object instance;
				if (TryGetService(type, out instance))
					return instance;

				throw new ActivationException("todo");
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception);
			}
		}

		public T GetService<T>()
		{
			try
			{
				return (T)GetService(typeof(T));
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception);
			}
		}

		public bool TryGetService(Type type, out object instance)
		{
			var removeStackFrame = false;

			try
			{
				AssertNotDisposed();
				AddStackFrame(type);
				removeStackFrame = true;

				return Kernel.TryGetService(type, this, out instance);
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception);
			}
			finally
			{
				if (removeStackFrame) RemoveStackFrame();
			}
		}

		public bool TryGetService<T>(out T instance)
		{
			try
			{
				object @object;
				if (TryGetService(typeof(T), out @object))
				{
					instance = (T)@object;
					return true;
				}

				instance = default(T);
				return false;
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception);
			}
		}

		public IEnumerable<object> GetServices(Type type)
		{
			try
			{
				var enumerableType = EnumerableTypeBuilder.Get(type);
				var services = GetService(enumerableType);
				return services as IEnumerable<object> ?? ((IEnumerable)services).Cast<object>();
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception);
			}
		}

		public IEnumerable<T> GetServices<T>()
		{
			try
			{
				return GetService<IEnumerable<T>>();
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception);
			}
		}

		private void AssertNotDisposed()
		{
			if (_disposed) throw new ObjectDisposedException(objectName: null, message: "Context has been disposed.");
		}

		private void AddStackFrame(Type type)
		{
			if (_processedTypes.Contains(type))
			{
				throw new InvalidOperationException($"Cyclic dependency, '{type.FullName}'.");
			}

			_processedTypes.Push(type);
		}

		private void RemoveStackFrame()
		{
			_processedTypes.Pop();
		}

		private static Exception CreateActivationException(Exception exception)
		{
			return new ActivationException($"todo : {exception.Message}", exception);
		}

		public void Dispose()
		{
			_disposed = true;
			Disposed?.Invoke(this);
		}

		public event Action<IContext> Disposed;
	}
}