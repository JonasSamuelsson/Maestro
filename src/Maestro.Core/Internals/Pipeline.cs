using Maestro.Lifetimes;

namespace Maestro.Internals
{
	class Pipeline : IPipeline
	{
		public Pipeline()
		{
		}

		public Pipeline(Plugin plugin)
		{
			Plugin = plugin;
		}

		public Plugin Plugin { get; set; }

		public object Execute(Context context)
		{
			var temp = new NextStep { Context = context, Pipeline = this };
			return Plugin.Lifetime.Execute(temp);
		}

		internal struct NextStep : INextStep
		{
			public Pipeline Pipeline { get; set; }
			public Context Context { get; set; }

			public object Execute()
			{
				return Pipeline.Plugin.FactoryProvider.GetFactory(Context).GetInstance(Context);
			}
		}
	}
}