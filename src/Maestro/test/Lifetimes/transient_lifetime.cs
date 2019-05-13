using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Maestro.Tests.Lifetimes
{
	public class transient_lifetime
	{
		[Fact]
		public void transient_should_be_the_default_lifetime_and_always_result_in_a_new_instance()
		{
			var container = new Container(x =>
													{
														x.Add<object>().Type<object>();
														x.Add<Parent>().Type<Parent>();
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

		[Fact]
		public void DisposableObjectsShouldBeDisposedAlongWithTheCorrespondingScope()
		{
			var strings = new List<string>();

			var container = new Container(x => x.Add<Disposable>().Self());

			var scope = container.CreateScope();

			var d1 = container.GetService<Disposable>();
			var d2 = scope.GetService<Disposable>();

			d1.Disposed.ShouldBeFalse();
			d2.Disposed.ShouldBeFalse();

			scope.Dispose();

			d1.Disposed.ShouldBeFalse();
			d2.Disposed.ShouldBeTrue();

			container.Dispose();

			d1.Disposed.ShouldBeTrue();
		}

		private class Disposable : IDisposable
		{
			public bool Disposed { get; set; }

			public void Dispose()
			{
				Disposed = true;
			}
		}
	}
}