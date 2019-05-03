using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Maestro.Utils
{
	class ThreadSafeList<T> : IEnumerable<T>
	{
		private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

		public int Count => _queue.Count;

		public void Add(T item)
		{
			_queue.Enqueue(item);
		}

		public void AddRange(IEnumerable<T> items)
		{
			items.ForEach(_queue.Enqueue);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _queue.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}