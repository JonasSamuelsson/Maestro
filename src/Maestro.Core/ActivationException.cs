using System;

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
			return $"Unable to get {n} service of type '{type.FullName}'.{Environment.NewLine}{reason}";
		}
	}
}