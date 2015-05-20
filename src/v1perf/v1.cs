using System.Collections.Generic;
using Maestro;

namespace v1perf
{
	public class v1 : PerformanceTest
	{
		private readonly Container _container = new Container(x =>
		{
			x.For<SingletonType>().Use<SingletonType>().Lifetime.Singleton();

			x.For<EnumerableType>().Use<EnumerableType>();
			x.For<EnumerableType>("foo").Use<EnumerableType>();
			x.For<EnumerableType>().Add<EnumerableType>();

			x.For<PropertyInjectionType>().Use<PropertyInjectionType>().Set(y => y.Dependency);
		});

		public override T Get<T>()
		{
			return _container.Get<T>();
		}

		protected override IEnumerable<T> GetAll<T>()
		{
			return _container.GetAll<T>();
		}
	}
}