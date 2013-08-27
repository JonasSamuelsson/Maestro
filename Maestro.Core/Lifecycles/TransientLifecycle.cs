namespace Maestro.Lifecycles
{
	internal class TransientLifecycle : ILifecycle
	{
		static TransientLifecycle()
		{
			Instance = new TransientLifecycle();
		}

		private TransientLifecycle() { }

		public static ILifecycle Instance { get; private set; }

		public ILifecycle Clone()
		{
			return Instance;
		}

		public object Execute(IContext context, IPipeline pipeline)
		{
			return pipeline.Execute();
		}
	}
}