namespace Maestro.Lifecycles
{
	public class SingletonLifecycle : LifecycleBase
	{
		private object _instance;

		public override object Execute(IContext context, IPipeline pipeline)
		{
			if (_instance == null)
				lock (this)
					if (_instance == null)
						_instance = pipeline.Execute();

			return _instance;
		}
	}
}