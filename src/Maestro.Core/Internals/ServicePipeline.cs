using Maestro.FactoryProviders.Factories;
using Maestro.Lifetimes;

namespace Maestro.Internals
{
	class ServicePipeline : IPipeline
	{
		public ServicePipeline(ServiceDescriptor serviceDescriptor, PipelineType pipelineType)
		{
			ServiceDescriptor = serviceDescriptor;
			PipelineType = pipelineType;
		}

		public ServiceDescriptor ServiceDescriptor { get; set; }
		public PipelineType PipelineType { get; }
		public IFactory Factory { get; set; }

		public object Execute(Context context)
		{
			var temp = new NextStep { Context = context, Pipeline = this };
			return ServiceDescriptor.Lifetime.Execute(temp);
		}

		internal struct NextStep : INextStep
		{
			public ServicePipeline Pipeline { get; set; }
			public Context Context { get; set; }

			public object Execute()
			{
				if (Pipeline.Factory == null)
				{
					Pipeline.Factory = Pipeline.ServiceDescriptor.FactoryProvider.GetFactory(Context);
				}

				var instance = Pipeline.Factory.GetInstance(Context);

				var interceptors = Pipeline.ServiceDescriptor.Interceptors;
				for (var i = 0; i < interceptors.Count; i++)
				{
					instance = interceptors[i].Execute(instance, Context);
				}

				return instance;
			}
		}
	}
}