namespace Maestro.Lifecycles
{
	internal class TransientLifecycle : LifecycleBase
	{
		static TransientLifecycle()
		{
			Instance = new TransientLifecycle();
		}

		private TransientLifecycle() { }

		public static ILifecycle Instance { get; private set; }

		public override ILifecycle Clone()
		{
			return Instance;
		}

		public override object Process(IContext context, IPipeline pipeline)
		{
			return pipeline.Execute();
		}
	}
}