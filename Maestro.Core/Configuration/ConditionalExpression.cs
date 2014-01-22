using System;

namespace Maestro.Configuration
{
	internal class ConditionalExpression<TPlugin> : IConditionalExpression<TPlugin>
	{
		private readonly DefaultSettings _defaultSettings;
		private readonly ConditionalInstanceBuilder _conditionalInstanceBuilder;

		public ConditionalExpression(DefaultSettings defaultSettings)
		{
			_defaultSettings = defaultSettings;
			_conditionalInstanceBuilder = new ConditionalInstanceBuilder();
		}

		public IConditionalInstanceExpression<TPlugin> If(Func<IContext, bool> predicate)
		{
			if (predicate == null) throw new ArgumentNullException();
			var instanceRegistrator = new InstanceRegistrator<TPlugin>(x => _conditionalInstanceBuilder.Add(predicate, x), _defaultSettings);
			return new ConditionalInstanceExpression<TPlugin>(instanceRegistrator);
		}

		public IConditionalInstanceExpression<TPlugin> Else
		{
			get
			{
				var instanceRegistrator = new InstanceRegistrator<TPlugin>(x => _conditionalInstanceBuilder.Add(x), _defaultSettings);
				return new ConditionalInstanceExpression<TPlugin>(instanceRegistrator);
			}
		}

		internal IInstanceBuilder GetPipeline(Action<IConditionalExpression<TPlugin>> action)
		{
			action(this);
			return _conditionalInstanceBuilder;
		}
	}
}