using Maestro.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	public class Context : IDisposable
	{
		private readonly HashSet<ServiceRequest> _serviceRequestStack = new HashSet<ServiceRequest>();
		private bool _disposed;
		private IServiceProvider _serviceProvider;

		internal Context(ScopedContainer container, Kernel kernel)
		{
			ScopedContainer = container;
			Kernel = kernel;
		}

		public IScopedContainer Container => ScopedContainer;
		internal ScopedContainer ScopedContainer { get; }
		internal Kernel Kernel { get; }

		public bool CanGetService(Type type)
		{
			return CanGetService(type, ServiceNames.Default);
		}

		public bool CanGetService(Type type, string name)
		{
			name = GetValueOrDefaultName(name);
			var serviceRequest = new ServiceRequest(type, name);

			try
			{
				AssertNotDisposed();
				AddStackFrame(serviceRequest);

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
				RemoveStackFrame(serviceRequest);
			}
		}

		public bool CanGetService<T>()
		{
			return CanGetService<T>(ServiceNames.Default);
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

		/// <summary>
		/// Gets the default instance of type <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public object GetService(Type type)
		{
			return GetService(type, ServiceNames.Default);
		}

		/// <summary>
		/// Gets an instance of type <paramref name="type"/> named <paramref name="name"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name">Uses the default instance if a named instance isn't found.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Gets the default instance of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetService<T>()
		{
			return GetService<T>(ServiceNames.Default);
		}

		/// <summary>
		/// Gets an instance of type <typeparamref name="T"/> named <paramref name="name"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name">Uses the default instance if a named instance isn't found.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Tries to get instance of type <typeparam name="T"/>.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public bool TryGetService<T>(out T instance)
		{
			return TryGetService(ServiceNames.Default, out instance);
		}

		/// <summary>
		/// Tries to get instance of type <typeparam name="T"/> named <paramref name="name"/>.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Tries to get instance of type <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		public bool TryGetService(Type type, out object instance)
		{
			return TryGetService(type, ServiceNames.Default, out instance);
		}

		/// <summary>
		/// Tries to get instance of type <paramref name="type"/> named <paramref name="name"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		public bool TryGetService(Type type, string name, out object instance)
		{
			name = GetValueOrDefaultName(name);
			var serviceRequest = new ServiceRequest(type, name);

			try
			{
				AssertNotDisposed();
				AddStackFrame(serviceRequest);

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
				RemoveStackFrame(serviceRequest);
			}
		}

		/// <summary>
		/// Gets all instances of type <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Gets all instances of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
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

		internal bool TryGetPipeline(Type type, string name, out Pipeline pipeline)
		{
			name = GetValueOrDefaultName(name);
			var serviceRequest = new ServiceRequest(type, name);

			try
			{
				// don't need to check if disposed
				AddStackFrame(serviceRequest);

				return Kernel.TryGetPipeline(type, name, this, out pipeline);
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
				RemoveStackFrame(serviceRequest);
			}
		}

		internal object GetService(Type type, string name, Pipeline pipeline)
		{
			try
			{
				return pipeline.Execute(this);
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

		//private void AddStackFrame(Type type, string name)
		//{
		//	var serviceRequest = new ServiceRequest(type, name);

		//	AddStackFrame(serviceRequest);
		//}

		private void AddStackFrame(ServiceRequest request)
		{
			if (_serviceRequestStack.Add(request))
				return;

			throw new InvalidOperationException("Cyclic dependency.");
		}

		private void RemoveStackFrame(ServiceRequest serviceRequest)
		{
			_serviceRequestStack.Remove(serviceRequest);
		}

		private static Exception CreateActivationException(Type type, string name, Exception exception)
		{
			return new ActivationException(type, name, exception);
		}

		public IServiceProvider ToServiceProvider()
		{
			return _serviceProvider ?? (_serviceProvider = new ContextServiceProvider(this));
		}

		public void Dispose()
		{
			_disposed = true;
		}
	}
}