using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	internal class Context : IContext, IDisposable
	{
		private readonly Stack<ServiceRequest> _serviceRequests = new Stack<ServiceRequest>();
		private bool _disposed;

		public Context(IContainer container, Kernel kernel)
		{
			Container = container;
			Kernel = kernel;
		}

		public IContainer Container { get; }
		public IEnumerable<ServiceRequest> CallStack => _serviceRequests;
		internal Kernel Kernel { get; }

		public bool CanGetService(Type type, string name)
		{
			var removeStackFrame = false;
			name = GetValueOrDefaultName(name);

			try
			{
				AssertNotDisposed();
				AddStackFrame(type, name);
				removeStackFrame = true;

				return Kernel.CanGetService(type, name, this);
			}
			catch (ActivationException exception)
			{
				if (exception.Type == type && exception.Name == name) throw;
				throw new ActivationException(type, name, exception);
			}
			catch (Exception exception)
			{
				throw CreateActivationException(type, name, exception);
			}
			finally
			{
				if (removeStackFrame) RemoveStackFrame();
			}
		}

		public bool CanGetService<T>(string name)
		{
			var type = typeof(T);
			name = GetValueOrDefaultName(name);

			try
			{
				return CanGetService(type, name);
			}
			catch (ActivationException exception)
			{
				if (exception.Type == type && exception.Name == name) throw;
				throw new ActivationException(type, name, exception);
			}
			catch (Exception exception)
			{
				throw CreateActivationException(type, name, exception);
			}
		}

		public object GetService(Type type)
		{
			return GetService(type, ServiceNames.Default);
		}

		public object GetService(Type type, string name)
		{
			name = GetValueOrDefaultName(name);

			try
			{
				object instance;
				if (TryGetService(type, name, out instance))
					return instance;

				throw new ActivationException(type, name, "Service not registered.");
			}
			catch (ActivationException exception)
			{
				if (exception.Type == type && exception.Name == name) throw;
				throw new ActivationException(type, name, exception);
			}
			catch (Exception exception)
			{
				throw CreateActivationException(type, name, exception);
			}
		}

		public T GetService<T>()
		{
			return GetService<T>(ServiceNames.Default);
		}

		public T GetService<T>(string name)
		{
			var type = typeof(T);
			name = GetValueOrDefaultName(name);

			try
			{
				return (T)GetService(type, name);
			}
			catch (ActivationException exception)
			{
				if (exception.Type == type && exception.Name == name) throw;
				throw new ActivationException(type, name, exception);
			}
			catch (Exception exception)
			{
				throw CreateActivationException(type, name, exception);
			}
		}

		public bool TryGetService<T>(out T instance)
		{
			return TryGetService(ServiceNames.Default, out instance);
		}

		public bool TryGetService<T>(string name, out T instance)
		{
			var type = typeof(T);
			name = GetValueOrDefaultName(name);

			try
			{
				object @object;
				if (TryGetService(type, name, out @object))
				{
					instance = (T)@object;
					return true;
				}

				instance = default(T);
				return false;
			}
			catch (ActivationException exception)
			{
				if (exception.Type == type && exception.Name == name) throw;
				throw new ActivationException(type, name, exception);
			}
			catch (Exception exception)
			{
				throw CreateActivationException(type, name, exception);
			}
		}

		public bool TryGetService(Type type, out object instance)
		{
			return TryGetService(type, ServiceNames.Default, out instance);
		}

		public bool TryGetService(Type type, string name, out object instance)
		{
			var removeStackFrame = false;
			name = GetValueOrDefaultName(name);

			try
			{
				AssertNotDisposed();
				AddStackFrame(type, name);
				removeStackFrame = true;

				return Kernel.TryGetService(type, name, this, out instance);
			}
			catch (ActivationException exception)
			{
				if (exception.Type == type && exception.Name == name) throw;
				throw new ActivationException(type, name, exception);
			}
			catch (Exception exception)
			{
				throw CreateActivationException(type, name, exception);
			}
			finally
			{
				if (removeStackFrame) RemoveStackFrame();
			}
		}

		public IEnumerable<object> GetServices(Type type)
		{
			var enumerableType = EnumerableTypeBuilder.Get(type);
			var name = ServiceNames.Default;

			try
			{
				var services = GetService(enumerableType, name);
				return services as IEnumerable<object> ?? ((IEnumerable)services).Cast<object>();
			}
			catch (ActivationException exception)
			{
				if (exception.Type == enumerableType && exception.Name == name) throw;
				throw new ActivationException(enumerableType, name, exception);
			}
			catch (Exception exception)
			{
				throw CreateActivationException(enumerableType, name, exception);
			}
		}

		public IEnumerable<T> GetServices<T>()
		{
			var type = typeof(IEnumerable<T>);
			var name = ServiceNames.Default;

			try
			{
				return GetService<IEnumerable<T>>(name);
			}
			catch (ActivationException exception)
			{
				if (exception.Type == type && exception.Name == name) throw;
				throw new ActivationException(type, name, exception);
			}
			catch (Exception exception)
			{
				throw CreateActivationException(type, name, exception);
			}
		}

		private static string GetValueOrDefaultName(string name)
		{
			return name ?? ServiceNames.Default;
		}

		private void AssertNotDisposed()
		{
			if (_disposed) throw new ObjectDisposedException(objectName: null, message: "Context has been disposed.");
		}

		private void AddStackFrame(Type type, string name)
		{
			var request = new ServiceRequest(type, name);

			if (_serviceRequests.Contains(request))
			{
				throw new InvalidOperationException("Cyclic dependency.");
			}

			_serviceRequests.Push(request);
		}

		private void RemoveStackFrame()
		{
			_serviceRequests.Pop();
		}

		private static Exception CreateActivationException(Type type, string name, Exception exception)
		{
			return new ActivationException(type, name, exception);
		}

		public void Dispose()
		{
			_disposed = true;
		}
	}
}