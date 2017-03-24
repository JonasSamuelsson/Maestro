using System;
using System.Collections.Generic;
using Maestro.Utils;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.Utils
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
			elementType.ShouldBeNull();
			Reflector.IsGenericEnumerable(typeof(ICollection<string>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<string>), out elementType).ShouldBeFalse();
			elementType.ShouldBeNull();
			Reflector.IsGenericEnumerable(typeof(ICollection<int>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<int>), out elementType).ShouldBeFalse();
			elementType.ShouldBeNull();
			Reflector.IsGenericEnumerable(typeof(ICollection<Class>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<Class>), out elementType).ShouldBeFalse();
			elementType.ShouldBeNull();
			Reflector.IsGenericEnumerable(typeof(ICollection<IInterface>)).ShouldBeFalse();
			Reflector.IsGenericEnumerable(typeof(ICollection<IInterface>), out elementType).ShouldBeFalse();
			elementType.ShouldBeNull();
		}

		private interface IInterface { }
		private class Class { }

		[Fact]
		public void IsGeneric()
		{
			Type genericTypeDefinition;
			Type[] genericArguments;

			Reflector.IsGeneric(typeof(IEnumerable<>), out genericTypeDefinition, out genericArguments).ShouldBeFalse();
			genericTypeDefinition.ShouldBeNull();
			genericArguments.ShouldBeNull();

			Reflector.IsGeneric(typeof(IEnumerable<object>), out genericTypeDefinition, out genericArguments).ShouldBeTrue();
			genericTypeDefinition.ShouldBe(typeof(IEnumerable<>));
			genericArguments.ShouldBe(new[] { typeof(object) });

			Reflector.IsGeneric(typeof(List<object>), out genericTypeDefinition, out genericArguments).ShouldBeTrue();
			genericTypeDefinition.ShouldBe(typeof(List<>));
			genericArguments.ShouldBe(new[] { typeof(object) });

			Reflector.IsGeneric(typeof(ListOfObject), out genericTypeDefinition, out genericArguments).ShouldBeFalse();
			genericTypeDefinition.ShouldBeNull();
			genericArguments.ShouldBeNull();
		}

		class ListOfObject : List<object> { }
	}
}