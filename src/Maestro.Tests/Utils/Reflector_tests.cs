using System;
using System.Collections.Generic;
using Maestro.Utils;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Utils
{
	public class Reflector_tests
	{
		[Fact]
		public void IsPrimitive_should_return_true_for_object_string_and_value_types()
		{
			Reflector.IsPrimitive(typeof(object)).ShouldBeTrue();
			Reflector.IsPrimitive(typeof(string)).ShouldBeTrue();
			Reflector.IsPrimitive(typeof(int)).ShouldBeTrue();

			Reflector.IsPrimitive(typeof(Class)).ShouldBeFalse();
			Reflector.IsPrimitive(typeof(IInterface)).ShouldBeFalse();
		}

		[Fact]
		public void IsGenericEnumerable_should_return_true_regardless_of_element_type()
		{
			Type elementType;

			Reflector.IsGenericEnumerable(typeof(IEnumerable<object>)).ShouldBeTrue();
			Reflector.IsGenericEnumerable(typeof(IEnumerable<object>), out elementType).ShouldBeTrue();
			elementType.ShouldBe(typeof(object));
			Reflector.IsGenericEnumerable(typeof(IEnumerable<string>)).ShouldBeTrue();
			Reflector.IsGenericEnumerable(typeof(IEnumerable<string>), out elementType).ShouldBeTrue();
			elementType.ShouldBe(typeof(string));
			Reflector.IsGenericEnumerable(typeof(IEnumerable<int>)).ShouldBeTrue();
			Reflector.IsGenericEnumerable(typeof(IEnumerable<int>), out elementType).ShouldBeTrue();
			elementType.ShouldBe(typeof(int));

			Reflector.IsGenericEnumerable(typeof(IEnumerable<Class>)).ShouldBeTrue();
			Reflector.IsGenericEnumerable(typeof(IEnumerable<Class>), out elementType).ShouldBeTrue();
			elementType.ShouldBe(typeof(Class));
			Reflector.IsGenericEnumerable(typeof(IEnumerable<IInterface>)).ShouldBeTrue();
			Reflector.IsGenericEnumerable(typeof(IEnumerable<IInterface>), out elementType).ShouldBeTrue();
			elementType.ShouldBe(typeof(IInterface));

			Reflector.IsGenericEnumerable(typeof(ICollection<object>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<object>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
			Reflector.IsGenericEnumerable(typeof(ICollection<string>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<string>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
			Reflector.IsGenericEnumerable(typeof(ICollection<int>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<int>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
			Reflector.IsGenericEnumerable(typeof(ICollection<Class>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<Class>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
			Reflector.IsGenericEnumerable(typeof(ICollection<IInterface>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<IInterface>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
		}

		[Fact]
		public void IsPrimitiveGenericEnumerable_should_return_true_if_element_type_is_primitive()
		{
			Type elementType;

			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<object>)).ShouldBeTrue();
			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<object>), out elementType).ShouldBeTrue();
			elementType.ShouldBe(typeof(object));
			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<string>)).ShouldBeTrue();
			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<string>), out elementType).ShouldBeTrue();
			elementType.ShouldBe(typeof(string));
			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<int>)).ShouldBeTrue();
			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<int>), out elementType).ShouldBeTrue();
			elementType.ShouldBe(typeof(int));

			Reflector.IsPrimitiveGenericEnumerable(typeof(ICollection<object>)).ShouldBeFalse();
			Reflector.IsPrimitiveGenericEnumerable(typeof(ICollection<object>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
			Reflector.IsPrimitiveGenericEnumerable(typeof(ICollection<string>)).ShouldBeFalse();
			Reflector.IsPrimitiveGenericEnumerable(typeof(ICollection<string>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
			Reflector.IsPrimitiveGenericEnumerable(typeof(ICollection<int>)).ShouldBeFalse();
			Reflector.IsPrimitiveGenericEnumerable(typeof(ICollection<int>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);

			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<Class>)).ShouldBeFalse();
			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<Class>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<IInterface>)).ShouldBeFalse();
			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<IInterface>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
		}

		[Fact]
		public void IsNonPrimitiveGenericEnumerable_should_return_true_if_element_type_isnt_primitive()
		{
			Type elementType;

			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<object>)).ShouldBeFalse();
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<object>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<string>)).ShouldBeFalse();
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<string>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<int>)).ShouldBeFalse();
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<int>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);

			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<Class>)).ShouldBeTrue();
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<Class>), out elementType).ShouldBeTrue();
			elementType.ShouldBe(typeof(Class));
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<IInterface>)).ShouldBeTrue();
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<IInterface>), out elementType).ShouldBeTrue();
			elementType.ShouldBe(typeof(IInterface));

			Reflector.IsNonPrimitiveGenericEnumerable(typeof(ICollection<Class>)).ShouldBeFalse();
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(ICollection<Class>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(ICollection<IInterface>)).ShouldBeFalse();
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(ICollection<IInterface>), out elementType).ShouldBeFalse();
			elementType.ShouldBe(null);
		}

		private interface IInterface { }
		private class Class { }
	}
}