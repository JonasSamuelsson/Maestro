using Maestro.FactoryProviders;
using Maestro.Lifetimes;

namespace Maestro.Internals
{
	class Pipeline : IPipeline
	{
		public Pipeline()
		{
		}

		public Pipeline(IPlugin plugin)
		{
			FactoryProvider = plugin.FactoryProvider;
		}

		public IFactoryProvider FactoryProvider { get; set; }
		public ILifetime Lifetime { get; set; }

		public object Execute(Context context)
		{
			return FactoryProvider.GetFactory(context).GetInstance(context);
		}
	}
}