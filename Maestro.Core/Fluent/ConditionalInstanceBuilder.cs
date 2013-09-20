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

		public IProviderSelector<TPlugin> If(Func<IContext, bool> predicate)
		{
			if (predicate == null) throw new ArgumentNullException();
			return new ProviderSelector<TPlugin>(x => _conditionalPipelineEngine.Add(predicate, x), _defaultSettings);
		}

		public IProviderSelector<TPlugin> Else
		{
			get { return new ProviderSelector<TPlugin>(x => _conditionalPipelineEngine.Add(x), _defaultSettings); }
		}

		internal IPipelineEngine GetPipeline(Action<IConditionalInstanceBuilder<TPlugin>> action)
		{
			action(this);
			return _conditionalPipelineEngine;
		}
	}
}