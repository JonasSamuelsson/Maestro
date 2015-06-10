using System.Collections.Generic;
using Maestro.FactoryProviders.Factories;
using Maestro.Interceptors;
using Maestro.Lifetimes;

namespace Maestro.Internals
{
	class Pipeline
	{
		public Pipeline()
		{
		}

		public Pipeline(Plugin plugin)
		{
			Plugin = plugin;
		}

		public Plugin Plugin { get; set; }
		public IFactory Factory { get; set; }

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
				if (Pipeline.Factory == null)
				{
					Pipeline.Factory = Pipeline.Plugin.FactoryProvider.GetFactory(Context);
				}

            var instance = Pipeline.Factory.GetInstance(Context);

				var interceptors = Pipeline.Plugin.Interceptors;
				for (var i = 0; i < interceptors.Count; i++)
				{
					instance = interceptors[i].Execute(instance, Context);
				}

				return instance;
			}
		}
	}
}