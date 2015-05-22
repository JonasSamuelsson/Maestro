namespace Maestro.Internals
{
	interface IPipeline
	{
		object Execute(Context context);
	}
}