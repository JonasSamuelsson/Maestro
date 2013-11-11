using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
				throw new InvalidOperationException("Cyclic dependency.");
			
			var index = types.Count;
			types.Add(type);
			return new Disposable(() => types.RemoveAt(index));
		}

		private struct Disposable : IDisposable
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