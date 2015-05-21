using Maestro.Internals.FactoryProviders;

namespace Maestro.Internals
{
	class Plugin : IPlugin
	{
		public IFactoryProvider FactoryProvider { get; set; }
	}
}