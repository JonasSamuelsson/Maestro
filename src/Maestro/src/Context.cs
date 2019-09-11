using Maestro.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	public class Context : IDisposable
	{
		private bool _disposed;
		private readonly DependencyDepthChecker _dependencyDepthChecker = new DependencyDepthChecker(100);
		private IServiceProvider _serviceProvider;

		internal Context(Kernel kernel, Container container, Scope scope)
		{
			Kernel = kernel;
			Container = container;
			Scope = scope;
		}

		internal Kernel Kernel { get; }

		public Container Container { get; }
		public Scope Scope { get; }

		public bool CanGetService(Type type)
		{
			return CanGetService(type, ServiceNames.Default);
		}

		public bool CanGetService(Type type, string name)
		{
			name = GetValueOrDefaultName(name);

			try
			{
				AssertNotDisposed();
				_dependencyDepthChecker.Push();

				return Kernel.CanGetService(type, name, this);
			}
			catch (ActivationException exception)
			{
				PopulateActivationExceptionMessage(exception, type, name);
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception, type, name);
			}
			finally
			{
				_dependencyDepthChecker.Pop();
			}
		}

		public bool CanGetService<T>()
		{
			return CanGetService(typeof(T), ServiceNames.Default);
		}

		public bool CanGetService<T>(string name)
		{
			return CanGetService(typeof(T), name);
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
			try
			{
				return GetServiceInternal(type, name);
			}
			catch (ActivationException exception)
			{
				PopulateActivationExceptionMessage(exception, type, name);
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception, type, name);
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

			try
			{
				return (T)GetServiceInternal(type, name);
			}
			catch (ActivationException exception)
			{
				PopulateActivationExceptionMessage(exception, type, name);
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception, type, name);
			}
		}

		private object GetServiceInternal(Type type, string name)
		{
			name = GetValueOrDefaultName(name);

			if (TryGetServiceInternal(type, name, out var instance))
				return instance;

			throw new InvalidOperationException("Service not registered.");
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

			try
			{
				if (TryGetServiceInternal(type, name, out var @object))
				{
					instance = (T)@object;
					return true;
				}

				instance = default(T);
				return false;
			}
			catch (ActivationException exception)
			{
				PopulateActivationExceptionMessage(exception, type, name);
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception, type, name);
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
			try
			{
				return TryGetServiceInternal(type, name, out instance);
			}
			catch (ActivationException exception)
			{
				PopulateActivationExceptionMessage(exception, type, name);
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception, type, name);
			}
			finally
			{
				_dependencyDepthChecker.Pop();
			}
		}

		private bool TryGetServiceInternal(Type type, string name, out object instance)
		{
			name = GetValueOrDefaultName(name);

			try
			{
				AssertNotDisposed();
				_dependencyDepthChecker.Push();

				return Kernel.TryGetService(type, name, this, out instance);
			}
			finally
			{
				_dependencyDepthChecker.Pop();
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
			const string name = ServiceNames.Default;

			try
			{
				var services = GetServiceInternal(enumerableType, name);
				return services as IEnumerable<object> ?? ((IEnumerable)services).Cast<object>();
			}
			catch (ActivationException exception)
			{
				PopulateActivationExceptionMessage(exception, type, name);
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception, type, name);
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
			const string name = ServiceNames.Default;

			try
			{
				var services = GetServiceInternal(typeof(IEnumerable<T>), name);
				return services as IEnumerable<T> ?? ((IEnumerable)services).Cast<T>();
			}
			catch (ActivationException exception)
			{
				PopulateActivationExceptionMessage(exception, type, name);
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception, type, name);
			}
		}

		internal bool TryGetPipeline(Type type, string name, out Pipeline pipeline)
		{
			name = GetValueOrDefaultName(name);

			try
			{
				// don't need to check if disposed
				_dependencyDepthChecker.Push();

				return Kernel.TryGetPipeline(type, name, this, out pipeline);
			}
			catch (ActivationException exception)
			{
				PopulateActivationExceptionMessage(exception, type, name);
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception, type, name);
			}
			finally
			{
				_dependencyDepthChecker.Pop();
			}
		}

		internal object ExecutePipeline(Pipeline pipeline, Type type, string name)
		{
			try
			{
				// don't need to check if disposed
				_dependencyDepthChecker.Push();

				return pipeline.Execute(this);
			}
			catch (ActivationException exception)
			{
				PopulateActivationExceptionMessage(exception, type, name);
				throw;
			}
			catch (Exception exception)
			{
				throw CreateActivationException(exception, type, name);
			}
			finally
			{
				_dependencyDepthChecker.Pop();
			}
		}

		private static string GetValueOrDefaultName(string name)
		{
			return name ?? ServiceNames.Default;
		}

		private void AssertNotDisposed()
		{
			if (_disposed) throw new ObjectDisposedException(null, "Context has been disposed.");
		}

		private static Exception CreateActivationException(Exception exception, Type type, string name)
		{
			return new ActivationException(exception, type, name);
		}

		private static void PopulateActivationExceptionMessage(ActivationException exception, Type type, string name)
		{
			exception.AddTraceFrame(type, name);
		}

		public IServiceProvider ToServiceProvider()
		{
			AssertNotDisposed();
			return _serviceProvider ?? (_serviceProvider = new ServiceProvider(TryGetService));
		}

		void IDisposable.Dispose()
		{
			_disposed = true;
		}
	}
}