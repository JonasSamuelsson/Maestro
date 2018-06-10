using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace Maestro.Tests.Benchmarks.BenchmarkDotNet
{
	[MemoryDiagnoser, SimpleJob(launchCount: 1, warmupCount: 5, targetCount: 20, invocationCount: 10_000)]
	public class DefaultBenchmarks
	{
		private static readonly IContainer Container = new Container(x =>
		{
			x.Use<Types.Transient>().Self();
			x.Use<Types.Scoped>().Self().Scoped();
			x.Use<Types.Singleton>().Self().Singleton();
			x.Use(typeof(Types.Generic<>)).Self();
			x.Use<Types.WithProperty>().Self().SetProperty(y => y.Dependency);
		});

		private static readonly IContainer ChildContainer = Container.GetChildContainer();

		[Benchmark(Baseline = true)]
		public void Baseline() => new Types.Transient();

		[Benchmark]
		public void Transient() => Container.GetService<Types.Transient>();

		[Benchmark]
		public static void Scoped() => Container.GetService<Types.Scoped>();

		[Benchmark]
		public static void Singleton() => Container.GetService<Types.Singleton>();

		[Benchmark]
		public static void ObjectGraph() => Container.GetService<Types.C3>();

		[Benchmark]
		public static void NewChildContainer()
		{
			using (var container = Container.GetChildContainer())
				container.GetService<Types.Transient>();
		}

		[Benchmark]
		public static void ReusedChildContainer() => ChildContainer.GetService<Types.Transient>();

		[Benchmark]
		public static void Generic() => Container.GetService<Types.Generic<object>>();

		[Benchmark]
		public static void Enumerable() => Container.GetServices<Types.Transient>();

		[Benchmark]
		public static void PropertyInjection() => Container.GetService<Types.WithProperty>();

		class Types
		{
			public class Transient { }

			public class Scoped { }

			public class Singleton { }

			public class Generic<T> { }

			public class WithProperty
			{
				public Transient Dependency { get; set; }
			}

			public class C0 { }
			public class C1 { public C1(C0 c0) { } }
			public class C2 { public C2(C1 c1, C0 c0) { } }
			public class C3 { public C3(C2 c2, C1 c1, C0 c0) { } }
		}
	}
}