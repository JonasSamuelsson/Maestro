using FluentAssertions;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Maestro.Tests
{
	public class add_concrete_sub_classes_convention
	{
		[Fact]
		public void should_register_concrete_sub_classes_of_provided_type()
		{
			var types = GetType().GetNestedTypes(BindingFlags.NonPublic);
			var container = new Container(x => x.Scan.Types(types).AddConcreteSubClassesOf<IBaseType>());

			var instances = container.GetAll<IBaseType>().ToList();

			instances.Should().HaveCount(2);
			instances.Should().Contain(x => x.IsOfType<Implementation1>());
			instances.Should().Contain(x => x.IsOfType<Implementation2>());
		}

		private interface IBaseType { }
		private class Implementation1 : IBaseType { }
		private class Implementation2 : IBaseType { }
	}
}