namespace Maestro
{
	internal class Pipeline : IPipeline
	{
		private readonly IProvider _provider;

		public Pipeline(IProvider provider)
		{
			_provider = provider;
		}

		public bool CanGet(IContext context)
		{
			return _provider.CanGet(context);
		}

		public object Get(IContext context)
		{
			return _provider.Get(context);
		}
	}
}