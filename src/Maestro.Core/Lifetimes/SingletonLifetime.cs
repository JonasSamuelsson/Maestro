namespace Maestro.Lifetimes
{
	internal class SingletonLifetime : ILifetime
	{
		private object _instance;

		public ILifetime Clone()
		{
			return new SingletonLifetime();
		}

		public object Execute(INextStep nextStep)
		{
			if (_instance == null)
				lock (this)
					if (_instance == null)
						_instance = nextStep.Execute();

			return _instance;
		}

		public override string ToString()
		{
			return "singleton";
		}
	}
}