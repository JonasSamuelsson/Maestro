using System;

namespace Maestro.Fluent
{
	internal class ConditionalInstanceBuilder<TPlugin> : IConditionalInstanceBuilder<TPlugin>
	{
		private readonly DefaultSettings _defaultSettings;
		private readonly ConditionalInstanceProvider _conditionalInstanceProvider;

		public ConditionalInstanceBuilder(DefaultSettings defaultSettings)
		{
			_defaultSettings = defaultSettings;
			_conditionalInstanceProvider = new ConditionalInstanceProvider();
		}

		public IProviderSelector<TPlugin> If(Func<IContext, bool> predicate)
		{
			return new ProviderSelector<TPlugin>(x => _conditionalInstanceProvider.Add(predicate, x), _defaultSettings);
		}

		public IProviderSelector<TPlugin> Else
		{
			get { return new ProviderSelector<TPlugin>(x => _conditionalInstanceProvider.Add(x), _defaultSettings); }
		}

		public IPipelineEngine GetPipeline(Action<IConditionalInstanceBuilder<TPlugin>> action)
		{
			action(this);
			return new PipelineEngine(_conditionalInstanceProvider);
		}
	}
}