using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class TypeStack : ITypeStack
	{
		private readonly List<Type> types = new List<Type>();

		public IEnumerator<Type> GetEnumerator()
		{
			return types.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Type Root
		{
			get { return types.Last(); }
		}

		public Type Current
		{
			get { return types.First(); }
		}

		public IDisposable Push(Type type)
		{
			if (types.Contains(type))
				throw new InvalidOperationException("Cyclic dependency.");

			types.Insert(0, type);
			return new Disposable(() => types.RemoveAt(0));
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