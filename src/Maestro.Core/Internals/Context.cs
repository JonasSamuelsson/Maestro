using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	class Context : IContext, IDisposable
	{
		private readonly Stack<ServiceRequest> _serviceRequests = new Stack<ServiceRequest>();
		private bool _disposed;

		public Context(string name, Kernel kernel)
		{
			Name = name ?? ServiceDescriptorLookup.DefaultName;
			Kernel = kernel;
		}

		public string Name { get; }
		public Kernel Kernel { get; }

		public bool CanGetService(Type type, string name)
		{
			var removeStackFrame = false;

			try
			{
				AssertNotDisposed();
				AddStackFrame(type);
				removeStackFrame = true;

				return Kernel.CanGetService(type, name, this);
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

		public bool CanGetService<T>(string name)
		{
			try
			{
				return CanGetService(typeof(T), name);
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

		public object GetService(Type type, string name)
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

		public T GetService<T>(string name)
		{
			try
			{
				return (T)GetService(typeof(T), name);
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

		public bool TryGetService<T>(out T instance)
		{
			return TryGetService(ServiceDescriptorLookup.DefaultName, out instance);
		}

		public bool TryGetService<T>(string name, out T instance)
		{
			try
			{
				object @object;
				if (TryGetService(typeof(T), name, out @object))
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

		public bool TryGetService(Type type, out object instance)
		{
			return TryGetService(type, ServiceDescriptorLookup.DefaultName, out instance);
		}

		public bool TryGetService(Type type, string name, out object instance)
		{
			var removeStackFrame = false;

			try
			{
				AssertNotDisposed();
				AddStackFrame(type);
				removeStackFrame = true;

				return Kernel.TryGetService(type, name, this, out instance);
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

		public IEnumerable<object> GetServices(Type type)
		{
			try
			{
				var enumerableType = EnumerableTypeBuilder.Get(type);
				var services = GetService(enumerableType, ServiceDescriptorLookup.DefaultName);
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
				return GetService<IEnumerable<T>>(ServiceDescriptorLookup.DefaultName);
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
			var request = new ServiceRequest(type, Name);

			if (_serviceRequests.Contains(request))
			{
				throw new InvalidOperationException($"Cyclic dependency, '{type.FullName}'.");
			}

			_serviceRequests.Push(request);
		}

		private void RemoveStackFrame()
		{
			_serviceRequests.Pop();
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