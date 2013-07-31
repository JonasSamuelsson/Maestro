namespace Maestro
{
	internal interface IPipeline
	{
		bool CanGet(IContext context);
		object Get(IContext context);
	}
}