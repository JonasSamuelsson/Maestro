using Castle.DynamicProxy;

namespace Maestro
{
	public struct Input<TIn>
	{
		public Input(TIn instance, IContext context, ProxyGenerator proxyGenerator) : this()
		{
			Instance = instance;
			Context = context;
			ProxyGenerator = proxyGenerator;
		}

		public TIn Instance { get; private set; }
		public IContext Context { get; private set; }
		public ProxyGenerator ProxyGenerator { get; set; }
	}
}