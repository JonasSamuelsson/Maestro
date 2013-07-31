namespace Maestro
{
	internal class Pipeline : IPipeline
	{
		private readonly IProvider _provider;

		public Pipeline(IProvider provider)
		{
			_provider = provider;
		}

		public object Get()
		{
			return _provider.Get();
		}
	}
}