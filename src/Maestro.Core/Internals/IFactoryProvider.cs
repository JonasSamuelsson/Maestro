using System;

namespace Maestro.Internals
{
	interface IFactoryProvider
	{
		Func<Context, object> GetFatory();
	}

	class TypeFactoryProvider : IFactoryProvider
	{
		public Type Type { get; set; }

		public Func<Context, object> GetFatory()
		{
			throw new NotImplementedException();
		}
	}

	class LambdaFactoryProvider : IFactoryProvider
	{
		private readonly Func<Context, object> _factory;

		public LambdaFactoryProvider(Func<Context, object> factory)
		{
			_factory = factory;
		}

		public Func<Context, object> GetFatory()
		{
			return _factory;
		}
	}

	class InstanceFactoryProvider : IFactoryProvider
	{
		private readonly Func<Context, object> _providerMethod;

		public InstanceFactoryProvider(object instance)
		{
			_providerMethod = _ => instance;
		}

		public Func<Context, object> GetFatory()
		{
			return _providerMethod;
		}
	}
}