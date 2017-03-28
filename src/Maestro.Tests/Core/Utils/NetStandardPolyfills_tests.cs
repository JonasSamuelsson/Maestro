using System;
using System.Linq;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.Utils
{
	public class NetStandardPolyfills_tests
	{
		[Fact]
		public void GetConstructors_should_only_return_instance_constructors()
		{
			var constructors = NetStandardPolyfills.GetConstructors(typeof(TestClass)).ToList();

			constructors.Count.ShouldBe(1);
			constructors.ShouldAllBe(x => x.IsStatic == false);
		}

		[Fact]
		public void GetMethod_should_get_method()
		{
			var instanceMethod = NetStandardPolyfills.GetMethod(typeof(TestClass), nameof(TestClass.InstanceMethod), new Type[] { });
			instanceMethod.Name.ShouldBe(nameof(TestClass.InstanceMethod));
			instanceMethod.GetParameters().ShouldBeEmpty();

			var staticMethod = NetStandardPolyfills.GetMethod(typeof(TestClass), nameof(TestClass.StaticMethod), new[] { typeof(int) });
			staticMethod.Name.ShouldBe(nameof(TestClass.StaticMethod));
			staticMethod.GetParameters().Single().ParameterType.ShouldBe(typeof(int));
		}

		[Fact]
		public void GetProperty_should_only_return_instance_properties()
		{
			NetStandardPolyfills.GetProperty(typeof(TestClass), nameof(TestClass.InstanceProperty)).ShouldNotBeNull();
			Should.Throw<InvalidOperationException>(() => NetStandardPolyfills.GetProperty(typeof(TestClass), nameof(TestClass.StaticProperty)));
		}

		class TestClass
		{
			static TestClass() { }

			public int InstanceProperty { get; set; }
			public static int StaticProperty { get; set; }

			public void InstanceMethod() { }
			public void InstanceMethod(int i) { }
			public static void StaticMethod() { }
			public static void StaticMethod(int i) { }
		}
	}
}