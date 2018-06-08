namespace Maestro.FactoryProviders.Factories
{
	interface IFactory
	{
		object GetInstance(Context context);
	}
}