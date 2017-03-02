using System;
using Maestro.Utils;

namespace Maestro
{
	public class ActivationException : Exception
	{
		public ActivationException(Type type, string name, string reason) : base(CreateMessage(type, name, reason))
		{
			Type = type;
			Name = name;
		}

		public ActivationException(Type type, string name, Exception innerException)
			: base(CreateMessage(type, name, innerException.Message), innerException)
		{
			Type = type;
			Name = name;
		}

		internal Type Type;
		internal string Name;

		private static string CreateMessage(Type type, string name, string reason)
		{
			var n = string.IsNullOrEmpty(name) ? "default" : $"'{name}'";
			var error = $"Could not get {n} service of type '{type.FullName}'.";
			return ExceptionMessageBuilder.GetMessage(error, reason);
		}
	}
}