using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core
{
	public abstract class primitive_type_tests
	{
		protected object ExpectedValue;
		protected Type PrimitiveType;

		[Fact]
		public void should_resolve_registered_instance()
		{
			var container = new Container(x => x.For(PrimitiveType).Use.Instance(ExpectedValue));

			var instance = container.GetService(PrimitiveType);

			instance.ShouldBe(ExpectedValue);
		}

		[Fact]
		public void should_not_resolve_unregistered_instance()
		{
			var container = new Container();

			Should.Throw<ActivationException>(() => container.GetService(PrimitiveType));
		}

		[Fact]
		public void should_resolve_all_with_no_instances_registered()
		{
			var container = new Container();

			var instance = container.GetServices(PrimitiveType);

			instance.ShouldBe(new object[] { });
		}

		[Fact]
		public void should_resolve_all_with_registered_instance()
		{
			var container = new Container(x => x.For(PrimitiveType).Add.Instance(ExpectedValue));

			var instance = container.GetServices(PrimitiveType);

			instance.ShouldBe(new[] { ExpectedValue });
		}

		public class resolve_struct : primitive_type_tests
		{
			public resolve_struct()
			{
				ExpectedValue = 1;
				PrimitiveType = typeof(int);
			}
		}

		public class resolve_string : primitive_type_tests
		{
			public resolve_string()
			{
				ExpectedValue = "foobar";
				PrimitiveType = typeof(string);
			}
		}

		public class resolve_object : primitive_type_tests
		{
			public resolve_object()
			{
				ExpectedValue = new object();
				PrimitiveType = typeof(object);
			}
		}
	}
}
