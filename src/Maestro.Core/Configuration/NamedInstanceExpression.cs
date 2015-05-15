using System;

namespace Maestro.Configuration
{
	internal class NamedInstanceExpression<TPlugin> : INamedInstanceExpression<TPlugin>
	{
		private readonly InstanceRegistrator<TPlugin> _registrator;

		public NamedInstanceExpression(InstanceRegistrator<TPlugin> registrator)
		{
			_registrator = registrator;
		}

		public void Use<TInstance>(TInstance instance) where TInstance : TPlugin
		{
			_registrator.Register(instance);
		}

		public IInstanceBuilderExpression<TInstance> Use<TInstance>(Func<TInstance> lambda) where TInstance : TPlugin
		{
			return _registrator.Register(lambda);
		}

		public IInstanceBuilderExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> lambda) where TInstance : TPlugin
		{
			return _registrator.Register(lambda);
		}

		public IInstanceBuilderExpression<TInstance> Use<TInstance>() where TInstance : TPlugin
		{
			return _registrator.Register<TInstance>();
		}

		public IInstanceBuilderExpression<TPlugin> Use(Type type)
		{
			return _registrator.Register(type);
		}
	}
}