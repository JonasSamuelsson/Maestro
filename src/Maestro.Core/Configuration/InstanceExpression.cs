using System;

namespace Maestro.Configuration
{
	internal class InstanceExpression<TPlugin> : IInstanceExpression<TPlugin>
	{
		private readonly InstanceRegistrator<TPlugin> _defaultInstanceRegistrator;
		private readonly InstanceRegistrator<TPlugin> _anonymousInstanceRegistrator;

		public InstanceExpression(InstanceRegistrator<TPlugin> defaultInstanceRegistrator, InstanceRegistrator<TPlugin> anonymousInstanceRegistrator)
		{
			_defaultInstanceRegistrator = defaultInstanceRegistrator;
			_anonymousInstanceRegistrator = anonymousInstanceRegistrator;
		}

		public void Use<TInstance>(TInstance instance) where TInstance : TPlugin
		{
			_defaultInstanceRegistrator.Register(instance);
		}

		public IInstanceBuilderExpression<TInstance> Use<TInstance>(Func<TInstance> lambda) where TInstance : TPlugin
		{
			return _defaultInstanceRegistrator.Register(lambda);
		}

		public IInstanceBuilderExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> lambda) where TInstance : TPlugin
		{
			return _defaultInstanceRegistrator.Register(lambda);
		}

		public IInstanceBuilderExpression<TInstance> Use<TInstance>() where TInstance : TPlugin
		{
			return _defaultInstanceRegistrator.Register<TInstance>();
		}

		public IInstanceBuilderExpression<TPlugin> Use(Type type)
		{
			return _defaultInstanceRegistrator.Register(type);
		}

		public void Add<TInstance>(TInstance instance) where TInstance : TPlugin
		{
			_anonymousInstanceRegistrator.Register(instance);
		}

		public IInstanceBuilderExpression<TInstance> Add<TInstance>(Func<TInstance> lambda) where TInstance : TPlugin
		{
			return _anonymousInstanceRegistrator.Register(lambda);
		}

		public IInstanceBuilderExpression<TInstance> Add<TInstance>(Func<IContext, TInstance> lambda) where TInstance : TPlugin
		{
			return _anonymousInstanceRegistrator.Register(lambda);
		}

		public IInstanceBuilderExpression<TInstance> Add<TInstance>() where TInstance : TPlugin
		{
			return _anonymousInstanceRegistrator.Register<TInstance>();
		}

		public IInstanceBuilderExpression<TPlugin> Add(Type type)
		{
			return _anonymousInstanceRegistrator.Register(type);
		}
	}
}