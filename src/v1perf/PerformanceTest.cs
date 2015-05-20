using System.Collections.Generic;

namespace v1perf
{
	public abstract class PerformanceTest
	{
		public TransientType Transient()
		{
			return Get<TransientType>();
		}

		public SingletonType Singleton()
		{
			return Get<SingletonType>();
		}

		public ComplexType Complex()
		{
			return Get<ComplexType>();
		}

		public GenericType<T> Generic<T>()
		{
			return Get<GenericType<T>>();
		}

		public PropertyInjectionType PropertyInjection()
		{
			return Get<PropertyInjectionType>();
		}

		public IEnumerable<EnumerableType> Enumerable()
		{
			return GetAll<EnumerableType>();
		}

		public abstract T Get<T>();
		protected abstract IEnumerable<T> GetAll<T>();

		public class TransientType { }
		public class SingletonType { }

		public class ComplexType
		{
			public ComplexType(TransientType transient, SingletonType singleton)
			{
				Transient = transient;
				Singleton = singleton;
			}

			public TransientType Transient { get; set; }
			public SingletonType Singleton { get; set; }
		}

		public class GenericType<T>
		{
			public GenericType(T dependency)
			{
				Dependency = dependency;
			}

			public T Dependency { get; set; }
		}

		public class PropertyInjectionType
		{
			public TransientType Dependency { get; set; }
		}

		public class EnumerableType { }
	}
}
