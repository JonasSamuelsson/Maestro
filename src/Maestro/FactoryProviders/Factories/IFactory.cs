namespace Maestro.FactoryProviders.Factories
{
	interface IFactory
	{
		object GetInstance(IContext context);
	}
}