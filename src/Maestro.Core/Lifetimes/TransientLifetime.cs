namespace Maestro.Lifetimes
{
	internal class TransientLifetime : ILifetime
	{
		static TransientLifetime()
		{
			Instance = new TransientLifetime();
		}

		public static ILifetime Instance { get; private set; }

		public ILifetime Clone()
		{
			return Instance;
		}

		public object Execute(INextStep nextStep)
		{
			return nextStep.Execute();
		}

		public override string ToString()
		{
			return "transient";
		}
	}
}