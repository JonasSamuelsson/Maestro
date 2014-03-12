using System;
using System.Text;

namespace Maestro.Utils
{
	internal class DiagnosticsBuilder
	{
		private readonly StringBuilder _builder = new StringBuilder();
		private int _indentations;
		private string _prefix = string.Empty;

		internal IDisposable Category(object o)
		{
			return Category("{0}", o);
		}

		internal IDisposable Category(string category)
		{
			Item(category);
			_indentations++;
			return new Disposable(() => _indentations--);
		}

		internal IDisposable Category(string format, params object[] args)
		{
			return Category(string.Format(format, args));
		}

		internal void Item(string item)
		{
			_builder.AppendLine(new string(' ', 3 * _indentations) + _prefix + item);
			_prefix = string.Empty;
		}

		internal void Item(string format, params object[] args)
		{
			Item(string.Format(format, args));
		}

		internal void Line()
		{
			Item(string.Empty);
		}

		internal void Prefix(string prefix)
		{
			_prefix = prefix;
		}

		internal void Prefix(string format, params object[] args)
		{
			Prefix(string.Format(format, args));
		}

		public override string ToString()
		{
			return _builder.ToString().Trim();
		}

		private class Disposable : IDisposable
		{
			private readonly Action _action;

			internal Disposable(Action action)
			{
				_action = action;
			}

			public void Dispose()
			{
				_action();
			}
		}
	}
}