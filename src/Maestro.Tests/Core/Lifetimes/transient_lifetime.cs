using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.Lifetimes
{
	public class transient_lifetime
	{
		[Fact]
		public void transient_should_be_the_default_lifetime_and_always_result_in_a_new_instance()
		{
			var container = new Container(x =>
													{
														x.Use<object>().Type<object>();
														x.Use<Parent>().Type<Parent>();
													});

			var instance = container.GetService<Parent>();

			instance.Object1.ShouldNotBe(instance.Object2);
		}

		private class Parent
		{
			public Parent(object object1, object object2)
			{
				Object1 = object1;
				Object2 = object2;
			}

			public object Object1 { get; private set; }
			public object Object2 { get; private set; }
		}
	}
}