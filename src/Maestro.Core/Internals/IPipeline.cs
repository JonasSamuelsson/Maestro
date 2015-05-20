namespace Maestro.Internals
{
	interface IPipeline
	{
		object Execute(Context context);
	}

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

		public object Execute(Context context)
		{
			return FactoryProvider.GetFactory(context).Invoke(context);
		}
	}
}