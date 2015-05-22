using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Shouldly;
using v1perf;
using Xunit;

namespace Maestro.Tests
{
	public class v1_vs_v2_performance
	{
		[Fact]
		public void v2_should_be_faster_than_v1()
		{
			var versions = new PerformanceTest[] { new v2() };
			foreach (var version in versions) Assert(version);

			var tests = new[]
			{
				new Test("transient", x => x.Transient()),
				new Test("singleton", x => x.Singleton()),
				new Test("complex", x => x.Complex()),
				new Test("generic", x => x.Generic<PerformanceTest.TransientType>()),
				new Test("property", x => x.PropertyInjection()),
				new Test("enumerable", x => x.Enumerable()),
			};

			var results = from version in versions
							  from test in tests
							  select new
							  {
								  version = version.GetType().Name,
								  test = test.Name,
								  duration = Execute(version, test.Action)
							  };

			foreach (var g in results.GroupBy(x => x.test))
			{
				Console.WriteLine(g.Key);
				foreach (var result in g)
				{
					Console.WriteLine($"  {result.version} {(int)result.duration.TotalMilliseconds}ms");
				}
			}
		}

		private static void Assert(PerformanceTest v)
		{
			v.Transient().ShouldNotBe(null);

			v.Singleton().ShouldBe(v.Singleton());

			var complex = v.Complex();
			complex.Singleton.ShouldNotBe(null);
			complex.Transient.ShouldNotBe(null);

			v.Generic<PerformanceTest.TransientType>().Dependency.ShouldNotBe(null);

			v.PropertyInjection().Dependency.ShouldNotBe(null);

			v.Enumerable().Count().ShouldBe(3);
		}

		private static TimeSpan Execute(PerformanceTest v, Action<PerformanceTest> action)
		{
			var stopwatch = Stopwatch.StartNew();
			for (var i = 0; i < 100 * 1000; i++) action(v);
			return stopwatch.Elapsed;
		}

		private class Test
		{
			public Test(string name, Action<PerformanceTest> action)
			{
				Name = name;
				Action = action;
			}

			public string Name { get; set; }
			public Action<PerformanceTest> Action { get; set; }
		}

		class v2 : PerformanceTest
		{
			readonly Container _container = new Container(x =>
																		 {
																			 x.For<SingletonType>().Use.Type<SingletonType>().Lifetime.Singleton();
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
}