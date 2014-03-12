namespace Maestro.Lifetimes
{
	internal class SingletonLifetime : ILifetime
	{
		private object _instance;

		public ILifetime Clone()
		{
			return new SingletonLifetime();
		}

		public object Execute(IContext context, IPipeline pipeline)
		{
			if (_instance == null)
				lock (this)
					if (_instance == null)
						_instance = pipeline.Execute();

			return _instance;
		}

		public override string ToString()
		{
			return "singleton";
		}
	}
}