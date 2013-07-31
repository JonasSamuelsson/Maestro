using System;

namespace Maestro.Fluent
{
	internal class PipelineSelector : IPipelineSelector
	{
		private readonly string _name;
		private readonly IPlugin _plugin;

		public PipelineSelector(string name, IPlugin plugin)
		{
			_name = name;
			_plugin = plugin;
		}

		public ITypePipelineBuilder Type(Type type)
		{
			var provider = new TypeProvider(type);
			var pipeline = new Pipeline(provider);
			_plugin.Add(_name, pipeline);
			return new TypePipelineBuilder(provider, pipeline);
		}
	}
}