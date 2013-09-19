using System;
using System.Text;

namespace Maestro
{
	public class ConfigOutputBuilder
	{
		private readonly StringBuilder _builder = new StringBuilder();
		private int _indentations;

		public IDisposable Category(string category)
		{
			Item(category);
			_indentations++;
			return new Disposable(() => _indentations--);
		}

		public IDisposable Category(string format, params object[] args)
		{
			return Category(string.Format(format, args));
		}

		public void Item(string item)
		{
			_builder.AppendLine(new string('\t', _indentations) + item);
		}

		public void Item(string format, params object[] args)
		{
			Item(string.Format(format, args));
		}

		public void Line()
		{
			Item(string.Empty);
		}

		public override string ToString()
		{
			return _builder.ToString().Trim();
		}

		private class Disposable : IDisposable
		{
			private readonly Action _action;

			public Disposable(Action action)
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