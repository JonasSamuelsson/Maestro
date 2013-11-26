using System;
using Maestro.Utils;

namespace Maestro.Factories
{
	internal class TypeInstanceFactory : IInstanceFactory
	{
		private readonly Type _type;
		private readonly ThreadSafeDictionary<Guid, Factory> _factories;

		public TypeInstanceFactory(Type type)
		{
			_type = type;
			_factories = new ThreadSafeDictionary<Guid, Factory>();
		}

		public bool CanGet(IContext context)
		{
			Factory factory;

			if (!_factories.TryGet(context.ContainerId, out factory) || factory.ConfigVersion != context.ConfigVersion)
			{
				factory = new Factory
									{
										ConfigVersion = context.ConfigVersion,
										Instantiate = Reflector.GetInstantiatorOrNull(_type, context)
									};
				_factories.Set(context.ContainerId, factory);
			}

			return factory.Instantiate != null;
		}

		public object Get(IContext context)
		{
			Factory factory;

			if (!_factories.TryGet(context.ContainerId, out factory) || factory.ConfigVersion != context.ConfigVersion)
			{
				factory = new Factory
									{
										ConfigVersion = context.ConfigVersion,
										Instantiate = Reflector.GetInstantiatorOrNull(_type, context)
									};
				_factories.Set(context.ContainerId, factory);
			}

			if (factory.Instantiate == null)
				throw new InvalidOperationException(string.Format("Can't find appropriate constructor for {0}.", _type.FullName));

			return factory.Instantiate(context);
		}

		public IInstanceFactory MakeGeneric(Type[] types)
		{
			var genericType = _type.MakeGenericType(types);
			return new TypeInstanceFactory(genericType);
		}

		public IInstanceFactory Clone()
		{
			return new TypeInstanceFactory(_type);
		}

		public override string ToString()
		{
			return string.Format("type instance : {0}", _type);
		}

		private class Factory
		{
			public int ConfigVersion { get; set; }
			public Func<IContext, object> Instantiate { get; set; }
		}
	}
}