using Maestro.FactoryProviders.Factories;

namespace Maestro.Internals
{
	class Pipeline : IPipeline
	{
		public Pipeline(ServiceDescriptor serviceDescriptor, PipelineType pipelineType)
		{
			ServiceDescriptor = serviceDescriptor;
			PipelineType = pipelineType;
		}

		public ServiceDescriptor ServiceDescriptor { get; set; }
		public PipelineType PipelineType { get; }
		public IFactory Factory { get; set; }

		public object Execute(Context context)
		{
			var temp = new NextStep { Pipeline = this };
			return ServiceDescriptor.Lifetime.Execute(context, temp.Execute);
		}

		internal struct NextStep
		{
			public Pipeline Pipeline { get; set; }

			public object Execute(IContext context)
			{
				var Context = (Context)context;

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