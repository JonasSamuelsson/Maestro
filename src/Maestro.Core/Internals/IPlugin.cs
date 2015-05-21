using Maestro.Internals.FactoryProviders;

namespace Maestro.Internals
{
	interface IPlugin
	{
		IFactoryProvider FactoryProvider { get; set; }
	}
}