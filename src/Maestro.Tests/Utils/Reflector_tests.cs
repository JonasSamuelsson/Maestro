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
			Reflector.IsGenericEnumerable(typeof(IEnumerable<object>)).ShouldBeTrue();
			Reflector.IsGenericEnumerable(typeof(IEnumerable<string>)).ShouldBeTrue();
			Reflector.IsGenericEnumerable(typeof(IEnumerable<int>)).ShouldBeTrue();

			Reflector.IsGenericEnumerable(typeof(IEnumerable<Class>)).ShouldBeTrue();
			Reflector.IsGenericEnumerable(typeof(IEnumerable<IInterface>)).ShouldBeTrue();

			Reflector.IsGenericEnumerable(typeof(ICollection<object>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<string>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<int>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<Class>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<IInterface>)).ShouldBeFalse();
		}

		[Fact]
		public void IsPrimitiveGenericEnumerable_should_return_true_if_element_type_is_primitive()
		{
			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<object>)).ShouldBeTrue();
			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<string>)).ShouldBeTrue();
			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<int>)).ShouldBeTrue();

			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<Class>)).ShouldBeFalse();
			Reflector.IsPrimitiveGenericEnumerable(typeof(IEnumerable<IInterface>)).ShouldBeFalse();
		}

		[Fact]
		public void IsNonPrimitiveGenericEnumerable_should_return_true_if_element_type_isnt_primitive()
		{
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<object>)).ShouldBeFalse();
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<string>)).ShouldBeFalse();
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<int>)).ShouldBeFalse();

			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<Class>)).ShouldBeTrue();
			Reflector.IsNonPrimitiveGenericEnumerable(typeof(IEnumerable<IInterface>)).ShouldBeTrue();
		}

		private interface IInterface { }
		private class Class { }
	}
}