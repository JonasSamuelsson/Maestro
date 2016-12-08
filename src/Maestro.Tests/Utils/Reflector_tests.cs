using System;
using System.Collections;
using System.Collections.Generic;
using Maestro.Utils;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Utils
{
	public class Reflector_tests
	{
		[Fact]
		public void should_return_false_if_type_isnt_IEnumerable()
		{
			Reflector.IsGenericEnumerable(typeof(string)).ShouldBe(false);
			Reflector.IsGenericEnumerable(typeof(IEnumerable)).ShouldBe(false);
			Reflector.IsGenericEnumerable(typeof(ICollection<object>)).ShouldBe(false);
			Reflector.IsGenericEnumerable(typeof(object[])).ShouldBe(false);
			Reflector.IsGenericEnumerable(typeof(IEnumerable<int>)).ShouldBe(false);
			Reflector.IsGenericEnumerable(typeof(IEnumerable<string>)).ShouldBe(false);
		}

		[Fact]
		public void should_return_true_if_type_is_IEnumerable()
		{
			Reflector.IsGenericEnumerable(typeof(IEnumerable<object>)).ShouldBe(true);
			Reflector.IsGenericEnumerable(typeof(IEnumerable<IDisposable>)).ShouldBe(true);
		}
	}
}