using System.Collections.Generic;
using FluentAssertions;
using Maestro.Interceptors;
using Maestro.Lifecycles;
using Xunit;

namespace Maestro.Tests
{
	public class open_generic_type
	{
		[Fact]
		public void configured_interceptors_and_lifecycles_should_be_cloned_and_clone_should_be_executed()
		{
			var createInterceptor = new Interceptor();
			var lifecycle = new Lifecycle();
			var activateInterceptor = new Interceptor();

			new Container(x => x.For(typeof(IList<>)).Use(typeof(List<>))
				.OnCreate.InterceptUsing(createInterceptor)
				.Lifecycle.Custom(lifecycle)
				.OnActivate.InterceptUsing(activateInterceptor))
				.Get<IList<int>>();

			createInterceptor.Cloned.Should().BeTrue();
			createInterceptor.Executed.Should().BeFalse();
			createInterceptor.Clone.Cloned.Should().BeFalse();
			createInterceptor.Clone.Executed.Should().BeTrue();

			lifecycle.Cloned.Should().BeTrue();
			lifecycle.Executed.Should().BeFalse();
			lifecycle.Clone.Cloned.Should().BeFalse();
			lifecycle.Clone.Executed.Should().BeTrue();

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

		private class Lifecycle : ILifecycle
		{
			public Lifecycle Clone { get; private set; }
			public bool Executed { get; private set; }

			public bool Cloned
			{
				get { return Clone != null; }
			}

			ILifecycle ILifecycle.Clone()
			{
				Clone = new Lifecycle();
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