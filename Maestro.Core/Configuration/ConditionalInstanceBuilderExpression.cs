using System;

namespace Maestro.Configuration
{
	internal class ConditionalInstanceBuilderExpression<TPlugin> : IConditionalInstanceBuilderExpression<TPlugin>
	{
		private readonly DefaultSettings _defaultSettings;
		private readonly ConditionalInstanceBuilder _conditionalInstanceBuilder;

		public ConditionalInstanceBuilderExpression(DefaultSettings defaultSettings)
		{
			_defaultSettings = defaultSettings;
			_conditionalInstanceBuilder = new ConditionalInstanceBuilder();
		}

		public IInstanceFactoryExpression<TPlugin> If(Func<IContext, bool> predicate)
		{
			if (predicate == null) throw new ArgumentNullException();
			return new InstanceFactoryExpression<TPlugin>(x => _conditionalInstanceBuilder.Add(predicate, x), _defaultSettings);
		}

		public IInstanceFactoryExpression<TPlugin> Else
		{
			get { return new InstanceFactoryExpression<TPlugin>(x => _conditionalInstanceBuilder.Add(x), _defaultSettings); }
		}

		internal IInstanceBuilder GetPipeline(Action<IConditionalInstanceBuilderExpression<TPlugin>> action)
		{
			action(this);
			return _conditionalInstanceBuilder;
		}
	}
}