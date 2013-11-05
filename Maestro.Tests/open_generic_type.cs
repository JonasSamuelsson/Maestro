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
			var createInterceptor = new Interceptor();
			var lifetime = new Lifetime();
			var activateInterceptor = new Interceptor();

			new Container(x => x.For(typeof(IList<>)).Use(typeof(List<>))
				.InterceptWith(createInterceptor)
				.Lifetime.Custom(lifetime)
				.InterceptWith(activateInterceptor))
				.Get<IList<int>>();

			createInterceptor.Cloned.Should().BeTrue();
			createInterceptor.Executed.Should().BeFalse();
			createInterceptor.Clone.Cloned.Should().BeFalse();
			createInterceptor.Clone.Executed.Should().BeTrue();

			lifetime.Cloned.Should().BeTrue();
			lifetime.Executed.Should().BeFalse();
			lifetime.Clone.Cloned.Should().BeFalse();
			lifetime.Clone.Executed.Should().BeTrue();

			activateInterceptor.Cloned.Should().BeTrue();
			activateInterceptor.Executed.Should().BeFalse();
			activateInterceptor.Clone.Cloned.Should().BeFalse();
			activateInterceptor.Clone.Executed.Should().BeTrue();
		}

		private class Interceptor : IInterceptor
		{
			public Interceptor Clone { get; private set; }
			public bool Executed { get; private set; }

			public bool Cloned
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

			public bool Cloned
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