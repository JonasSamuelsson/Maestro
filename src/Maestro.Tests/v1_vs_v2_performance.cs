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
			var versions = new PerformanceTest[] { new v1() };
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

			throw new Exception(string.Join(Environment.NewLine, results.Select(x => $"{x.version} {x.test} {(int)x.duration.TotalMilliseconds}ms")));
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
	}
}