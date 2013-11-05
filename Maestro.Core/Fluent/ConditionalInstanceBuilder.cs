using System;

namespace Maestro.Fluent
{
	internal class ConditionalInstanceBuilder<TPlugin> : IConditionalInstanceBuilder<TPlugin>
	{
		private readonly DefaultSettings _defaultSettings;
		private readonly ConditionalPipelineEngine _conditionalPipelineEngine;

		public ConditionalInstanceBuilder(DefaultSettings defaultSettings)
		{
			_defaultSettings = defaultSettings;
			_conditionalPipelineEngine = new ConditionalPipelineEngine();
		}

		public IInstanceExpression<TPlugin> If(Func<IContext, bool> predicate)
		{
			if (predicate == null) throw new ArgumentNullException();
			return new InstanceExpression<TPlugin>(x => _conditionalPipelineEngine.Add(predicate, x), _defaultSettings);
		}

		public IInstanceExpression<TPlugin> Else
		{
			get { return new InstanceExpression<TPlugin>(x => _conditionalPipelineEngine.Add(x), _defaultSettings); }
		}

		internal IPipelineEngine GetPipeline(Action<IConditionalInstanceBuilder<TPlugin>> action)
		{
			action(this);
			return _conditionalPipelineEngine;
		}
	}
}