using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Maestro.Tests
{
	public class concrete_classes_closing_convention
	{
		[Fact]
		public void should_register_type_closing_provided_generic_type_definition()
		{
			var types = new[] { typeof(List<IDisposable>), typeof(ListOfObjects) };
			var container = new Container(x => x.Scan.Types(types).ForConcreteClassesClosing(typeof(IList<>)));

			container.Invoking(x => x.Get<IList<IDisposable>>()).ShouldNotThrow("IList<IDisposable>");
			container.Invoking(x => x.Get<IList<object>>()).ShouldNotThrow("IList<object>");
			container.Invoking(x => x.Get<IList<string>>()).ShouldThrow<ActivationException>();
		}

		private class ListOfObjects : List<object> { }
	}
}