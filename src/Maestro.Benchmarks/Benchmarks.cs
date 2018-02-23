using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace Maestro.Benchmarks
{
	[SimpleJob(launchCount: 1, warmupCount: 5, targetCount: 20, invocationCount: 50_000)]
	public class Benchmarks
	{
		private static readonly IContainer Container = new Container(x =>
		{
			x.Use<Types.Transient>().Self();
			x.Use<Types.Scoped>().Self().Lifetime.ContainerScoped();
			x.Use<Types.Singleton>().Self().Lifetime.Singleton();
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
		public static void NewChildContainer() => Container.GetChildContainer().GetService<Types.Transient>();

		[Benchmark]
		public static void ReusedChildContainer() => ChildContainer.GetService<Types.Transient>();

		[Benchmark]
		public static void Generic() => Container.GetService<Types.Generic<object>>();

		[Benchmark]
		public static void Enumerable() => Container.GetServices<Types.Transient>();

		[Benchmark]
		public static void PropertyInjection() => Container.GetService<Types.WithProperty>();
	}
}