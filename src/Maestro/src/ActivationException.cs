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
			_messageBuilder.AppendLine($" -> type: {type.FullName} name: '{name ?? ServiceNames.Default}'");
		}
	}
}