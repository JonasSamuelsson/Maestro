namespace Maestro.Configuration
{
	public static class Extensions
	{
		public static void AddDiagnostics(this IContainerBuilder builder)
		{
			builder.Add<Diagnostics.Diagnostics>().Factory(ctx => new Diagnostics.Diagnostics(ctx.Kernel));
		}
	}
}