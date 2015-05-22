using System.Collections.Generic;
using FluentAssertions;
using Maestro.Interceptors;
using Maestro.Lifetimes;
using Xunit;

namespace Maestro.Tests
{
	public class open_generic_type
	{
		[Fact]
		public void configured_interceptors_and_lifetimes_should_be_cloned_and_clone_should_be_executed()
		{
			var lifetime = new Lifetime();
			var interceptor = new Interceptor();

			new Container(x => x.For(typeof(IList<>)).Use(typeof(List<>))
				.Lifetime.Use(lifetime)
				.Intercept(interceptor))
				.Get<IList<int>>();

			lifetime.IsCloned.Should().BeTrue();
			lifetime.Executed.Should().BeFalse();
			lifetime.Clone.IsCloned.Should().BeFalse();
			lifetime.Clone.Executed.Should().BeTrue();

			interceptor.IsCloned.Should().BeTrue();
			interceptor.Executed.Should().BeFalse();
			interceptor.Clone.IsCloned.Should().BeFalse();
			interceptor.Clone.Executed.Should().BeTrue();
		}

		private class Interceptor : IInterceptor
		{
			public Interceptor Clone { get; private set; }
			public bool Executed { get; private set; }

			public bool IsCloned
			{
				get { return Clone != null; }
			}

			IInterceptor IInterceptor.Clone()
			{
				Clone = new Interceptor();
				return Clone;
			}

			public object Execute(object instance, IContext context)
			{
				Executed = true;
				return instance;
			}
		}

		private class Lifetime : ILifetime
		{
			public Lifetime Clone { get; private set; }
			public bool Executed { get; private set; }

			public bool IsCloned
			{
				get { return Clone != null; }
			}

			ILifetime ILifetime.Clone()
			{
				Clone = new Lifetime();
				return Clone;
			}

			public object Execute(IContext context, IPipeline pipeline)
			{
				Executed = true;
				return pipeline.Execute();
			}
		}
	}
}