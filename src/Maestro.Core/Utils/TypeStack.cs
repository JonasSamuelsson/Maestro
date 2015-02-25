using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maestro.Utils
{
	internal class TypeStack : ITypeStack
	{
		private readonly List<Type> types = new List<Type>();

		public IEnumerator<Type> GetEnumerator()
		{
			return Enumerable.Reverse(types).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Type Root
		{
			get { return types[0]; }
		}

		public Type Current
		{
			get { return types[types.Count - 1]; }
		}

		public IDisposable Push(Type type)
		{
			if (types.Contains(type))
			{
				var builder = new StringBuilder();
				builder.AppendLine("Cyclic dependency.")
						 .AppendFormat("  {0}", type.FullName)
						 .AppendLine();
				foreach (var x in types.AsEnumerable().Reverse())
					builder.AppendFormat("  {0}", x.FullName).AppendLine();
				throw new InvalidOperationException(builder.ToString().Trim());
			}

			var index = types.Count;
			types.Add(type);
			return new Disposable(types, index);
		}

		private struct Disposable : IDisposable
		{
			private readonly List<Type> _list;
			private readonly int _index;

			public Disposable(List<Type> list, int index)
			{
				_list = list;
				_index = index;
			}

			public void Dispose()
			{
				_list.RemoveAt(_index);
			}
		}
	}
}