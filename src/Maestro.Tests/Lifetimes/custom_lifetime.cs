using System;
using Maestro.Lifetimes;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Lifetimes
{
	public class custom_lifetime
	{
		[Fact]
		public void provided_lifetime_should_be_executed()
		{
			var lifetime = new Lifetime();
			var container = new Container(x => x.For<object>().Use<object>().Lifetime.Use(lifetime));

			container.Get<object>();

			lifetime.Executed.ShouldBe(true);
		}

		private class Lifetime : ILifetime
		{
			public bool Executed { get; private set; }

			public ILifetime Clone()
			{
				throw new NotImplementedException();
			}

			public object Execute(INextStep nextStep)
			{
				Executed = true;
				return nextStep.Execute();
			}
		}
	}
}