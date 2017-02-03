using System.Collections;
using System.Collections.Generic;

namespace Maestro.Utils
{
	class ThreadSafeList<T> : IEnumerable<T>
	{
		private readonly object _root = new object();
		private List<T> _list = new List<T>();

		public int Count => _list.Count;

		public void Add(T item)
		{
			lock (_root)
			{
				_list = new List<T>(_list) { item };
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}