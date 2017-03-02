using System;

namespace Maestro.Utils
{
	internal static class ExceptionMessageBuilder
	{
		public static string GetMessage(string error, string reason)
		{
			return string.IsNullOrWhiteSpace(reason)
				? error
				: $"{error}{Environment.NewLine} > {reason}";
		}
	}
}