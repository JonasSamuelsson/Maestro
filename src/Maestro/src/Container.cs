using Maestro.Configuration;
using Maestro.Internals;
using System;

namespace Maestro
{
	public class Container : Scope, IContainer
	{
		/// <summary>
		/// Instantiates a new empty container.
		/// </summary>
		public Container()
			: base(new Kernel(), new WeakRefTransientDisposableTracker())
		{
		}

		/// <summary>
		/// Instantiates a new container with configuration.
		/// </summary>
		public Container(Action<IContainerBuilder> action)
			: this()
		{
			Configure(action);
		}

		public void Configure(Action<IContainerBuilder> action)
		{
			AssertNotDisposed();
			action.Invoke(new InternalContainerBuilder(this));
		}

		public IScope CreateScope()
		{
			AssertNotDisposed();
			return new ChildScope(Kernel, this);
		}

		protected override Context CreateContext()
		{
			return new Context(Kernel, this, this);
		}

		public override void Dispose()
		{
			Kernel.Dispose();
			base.Dispose();
		}
	}
}