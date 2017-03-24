using System;
using Maestro.Utils;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.Utils
{
	public class GenericInstanceFactory_tests
	{
		[Fact]
		public void ShouldCallMakeGenericMethod()
		{
			var types = new[] { typeof(bool), typeof(int) };
			GenericInstanceFactory.Create<object>(new TypeWithMakeGenericMethod(), types).ShouldBe(types);
		}

		[Fact]
		public void ShouldCallParameterlessCtor()
		{
			var object1 = new TypeWithParameterlessCtor();
			var object2 = GenericInstanceFactory.Create<object>(object1, new Type[] { });
			object2.ShouldBeOfType<TypeWithParameterlessCtor>();
			object1.ShouldNotBe(object2);
		}

		[Fact]
		public void ShouldThrowIfTypeDoesntHaveParameterlessCtorOrMakeGenericMethod()
		{
			var source = new TypeWithoutParameterlessCtorAndMakeGenericMethod(null);
			Should.Throw<InvalidOperationException>(() => GenericInstanceFactory.Create<object>(source, new Type[] { }));
		}

		private class TypeWithMakeGenericMethod
		{
			public object MakeGeneric(Type[] types)
			{
				return types;
			}
		}

		private class TypeWithParameterlessCtor { }

		private class TypeWithoutParameterlessCtorAndMakeGenericMethod
		{
			public TypeWithoutParameterlessCtorAndMakeGenericMethod(object o) { }
		}
	}
}
