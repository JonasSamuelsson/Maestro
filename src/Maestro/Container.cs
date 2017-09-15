﻿using Maestro.Configuration;
using Maestro.Diagnostics;
using Maestro.Internals;
using System;
using System.Collections.Generic;

namespace Maestro
{
	public class Container : IContainer
	{
		private readonly Guid _id;
		private readonly Kernel _kernel;
		private event Action<Guid> DisposedEvent;

		/// <summary>
		/// Instantiates a new empty container.
		/// </summary>
		public Container() : this(new Kernel())
		{ }

		/// <summary>
		/// Instantiates a new container with configuration.
		/// </summary>
		public Container(Action<IContainerExpression> action)
			: this()
		{
			Configure(action);
		}

		/// <summary>
		/// Instantiates a new container with configuration.
		/// </summary>
		public Container(ContainerBuilder builder)
			: this()
		{
			builder.Configure(this);
		}

		private Container(Kernel kernel)
		{
			_id = Guid.NewGuid();
			_kernel = kernel;
		}

		public IDiagnostics Diagnostics => new Diagnostics.Diagnostics(_kernel);

		public void Configure(Action<IContainerExpression> action)
		{
			using (var containerExpression = new ContainerExpression(_kernel))
				action(containerExpression);
		}

		public void Configure(ContainerBuilder builder)
		{
			builder.Configure(this);
		}

		public IContainer GetChildContainer()
		{
			return GetChildContainer(delegate { });
		}

		public IContainer GetChildContainer(Action<IContainerExpression> action)
		{
			var childContainer = new Container(new Kernel(_kernel));
			childContainer.Configure(action);
			return childContainer;
		}

		public IContainer GetChildContainer(ContainerBuilder builder)
		{
			var childContainer = GetChildContainer();
			builder.Configure(childContainer);
			return childContainer;
		}

		public object GetService(Type type)
		{
			using (var context = new Context(this, _kernel))
				return context.GetService(type, ServiceNames.Default);
		}

		public object GetService(Type type, string name)
		{
			using (var context = new Context(this, _kernel))
				return context.GetService(type, name);
		}

		public T GetService<T>()
		{
			using (var context = new Context(this, _kernel))
				return context.GetService<T>(ServiceNames.Default);
		}

		public T GetService<T>(string name)
		{
			using (var context = new Context(this, _kernel))
				return context.GetService<T>(name);
		}

		public bool TryGetService(Type type, out object instance)
		{
			using (var context = new Context(this, _kernel))
				return context.TryGetService(type, out instance);
		}

		public bool TryGetService(Type type, string name, out object instance)
		{
			using (var context = new Context(this, _kernel))
				return context.TryGetService(type, name, out instance);
		}

		public bool TryGetService<T>(out T instance)
		{
			using (var context = new Context(this, _kernel))
				return context.TryGetService(out instance);
		}

		public bool TryGetService<T>(string name, out T instance)
		{
			using (var context = new Context(this, _kernel))
				return context.TryGetService(name, out instance);
		}

		public IEnumerable<object> GetServices(Type type)
		{
			using (var context = new Context(this, _kernel))
				return context.GetServices(type);
		}

		public IEnumerable<T> GetServices<T>()
		{
			using (var context = new Context(this, _kernel))
				return context.GetServices<T>();
		}

		public event Action<Guid> Disposed
		{
			add { DisposedEvent += value; }
			remove { DisposedEvent -= value; }
		}

		public void Dispose()
		{
			DisposedEvent?.Invoke(_id);
			_kernel.Dispose();
		}
	}
}