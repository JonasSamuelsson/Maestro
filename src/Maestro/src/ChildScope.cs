using Maestro.Internals;

namespace Maestro
{
	internal class ChildScope : Scope
	{
		private readonly Kernel _kernel;
		private readonly Container _container;

		public ChildScope(Kernel kernel, Container container)
			: base(kernel, new StrongRefTransientDisposableTracker())
		{
			_kernel = kernel;
			_container = container;
		}

		protected override Context CreateContext()
		{
			return new Context(_kernel, _container, this);
		}
	}
}