using System.Linq;
using Maestro.Internals;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Internals
{
	public class ConstructorProviderTests
	{
		[Fact]
		public void Should_get_public_and_internal_constructors()
		{
			var ctors = new ConstructorProvider().GetConstructors(typeof(AccessModifiers)).ToList();

			ctors.Count.ShouldBe(2);
			ctors.ShouldContain(x => x.IsAssembly);
			ctors.ShouldContain(x => x.IsPublic);
		}

		[Fact]
		public void Should_get_instance_constructors()
		{
			new ConstructorProvider().GetConstructors(typeof(InstanceVsStatic)).Single().IsStatic.ShouldBeFalse();
		}

		[Fact]
		public void Should_order_constructors_by_param_count_descending()
		{
			var ctors = new ConstructorProvider().GetConstructors(typeof(SortOrder)).ToList();

			ctors.Count.ShouldBe(3);
			ctors[0].GetParameters().Length.ShouldBe(2);
			ctors[1].GetParameters().Length.ShouldBe(1);
			ctors[2].GetParameters().Length.ShouldBe(0);
		}

		private class AccessModifiers
		{
			internal AccessModifiers(Internal _) { }
			private AccessModifiers(Private _) { }
			protected AccessModifiers(Protected _) { }
			protected internal AccessModifiers(ProtectedInternal _) { }
			public AccessModifiers(Public _) { }

			public class Internal { }
			public class Private { }
			public class Protected { }
			public class ProtectedInternal { }
			public class Public { }
		}

		private class InstanceVsStatic
		{
			static InstanceVsStatic() { }
			public InstanceVsStatic() { }
		}

		private class SortOrder
		{
			public SortOrder() { }
			public SortOrder(int _, int __) { }
			public SortOrder(int _) { }
		}
	}
}