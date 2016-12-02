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
			Reflector.IsEnumerable(typeof(string)).ShouldBe(false);
			Reflector.IsEnumerable(typeof(IEnumerable)).ShouldBe(false);
			Reflector.IsEnumerable(typeof(ICollection<object>)).ShouldBe(false);
			Reflector.IsEnumerable(typeof(object[])).ShouldBe(false);
			Reflector.IsEnumerable(typeof(IEnumerable<int>)).ShouldBe(false);
			Reflector.IsEnumerable(typeof(IEnumerable<string>)).ShouldBe(false);
		}

		[Fact]
		public void should_return_true_if_type_is_IEnumerable()
		{
			Reflector.IsEnumerable(typeof(IEnumerable<object>)).ShouldBe(true);
			Reflector.IsEnumerable(typeof(IEnumerable<IDisposable>)).ShouldBe(true);
		}
	}
}