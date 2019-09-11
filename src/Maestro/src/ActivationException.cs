using System;
using System.Collections.Generic;
using System.Text;
using Maestro.Internals;

namespace Maestro
{
	public class ActivationException : Exception
	{
		private readonly StringBuilder _builder = new StringBuilder();
		private readonly List<string> _infos = new List<string>();

		public ActivationException(Exception innerException, Type type, string name)
			: base(innerException.Message, innerException)
		{
			_builder.AppendLine(innerException.Message);
			AddTraceFrame(type, name);
		}

		public override string Message => _builder.ToString().Trim();

		internal void AddTraceFrame(Type type, string name)
		{
			name = name ?? ServiceNames.Default;

			_builder.AppendLine($" -> service type: {type.ToFormattedString()}");

			if (name != ServiceNames.Default)
			{
				_builder.AppendLine($"   service name: {name}");
			}

			foreach (var info in _infos)
			{
				_builder.AppendLine($"    {info}");
			}

			_infos.Clear();
		}

		internal void AddTraceFrameInfo(string info)
		{
			_infos.Add(info);
		}
	}
}