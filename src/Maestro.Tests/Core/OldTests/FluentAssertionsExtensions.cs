namespace Maestro.Tests.Core
{
	internal static class FluentAssertionsExtensions
	{
		public static bool IsAssignableTo<T>(this object o)
		{
			return o is T;
		}

		public static bool IsOfType<T>(this object o)
		{
			return o.GetType() == typeof(T);
		}
	}
}