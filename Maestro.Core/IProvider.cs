namespace Maestro
{
	internal interface IProvider
	{
		bool CanGet(IContext context);
		object Get(IContext context);
	}
}