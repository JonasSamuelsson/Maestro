namespace Maestro.Benchmarks
{
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