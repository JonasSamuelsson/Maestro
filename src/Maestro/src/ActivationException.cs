using Maestro.Internals;
using System;
using System.Text;

namespace Maestro
{
	public class ActivationException : Exception
	{
		private readonly StringBuilder _messageBuilder = new StringBuilder();

		public ActivationException(Exception innerException, Type type, string name)
			: base(innerException.Message, innerException)
		{
			_messageBuilder.AppendLine(innerException.Message);
			AddToMessageTrace(type, name);
		}

		public override string Message => _messageBuilder.ToString().Trim();

		internal void AddToMessageTrace(Type type, string name)
		{
			name = name ?? ServiceNames.Default;

			var message = name == ServiceNames.Default
				? $" -> type: {type.FullName}"
				: $" -> type: {type.FullName} name: '{name}'";

			_messageBuilder.AppendLine(message);
		}
	}
}