namespace Maestro.Lifecycles
{
	public class SingletonLifecycle : ILifecycle
	{
		private object _instance;

		public ILifecycle Clone()
		{
			return new SingletonLifecycle();
		}

		public object Execute(IContext context, IPipeline pipeline)
		{
			if (_instance == null)
				lock (this)
					if (_instance == null)
						_instance = pipeline.Execute();

			return _instance;
		}
	}
}