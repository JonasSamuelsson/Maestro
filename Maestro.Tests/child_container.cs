using System;
using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class child_container
	{
		[Fact]
		public void should_get_instance_registered_in_child_container()
		{
			var value = "foobar";
			var parentContainer = new Container();
			var childContainer = parentContainer.GetChildContainer(x => x.For<string>().Use(value));
			childContainer.Get<string>().Should().Be(value);
		}

		[Fact]
		public void should_get_instance_registered_in_parent_container()
		{
			var value = "foobar";
			var parentContainer = new Container(x => x.For<string>().Use(value));
			var childContainer = parentContainer.GetChildContainer();
			childContainer.Get<string>().Should().Be(value);
		}

		[Fact]
		public void should_get_default_instance_if_not_registered_in_any_container()
		{
			new Container().GetChildContainer().Get<object>().Should().NotBeNull();
		}

		[Fact]
		public void should_get_dependencies_from_the_right_container()
		{
			var parentText = "parent";
			var childText = "child";

			var parentContainer = new Container(x => x.For<string>().Use(parentText));
			var childContainer = parentContainer.GetChildContainer(x => x.For<string>().Use(childText));

			parentContainer.Get<ParentObject>().Text.Should().Be(parentText);
			childContainer.Get<ParentObject>().Text.Should().Be(childText);
		}

		[Fact]
		public void child_containers_should_use_container_singleton_as_default_lifetime()
		{
			var container = new Container().GetChildContainer(x => x.For<object>().Use<object>());
			var o1 = container.Get<object>();
			var o2 = container.Get<object>();
			o1.Should().Be(o2);
		}

		[Fact]
		public void disposable_instances_in_child_container_should_be_disposed_with_the_container()
		{
			DisposableObject o;
			using (var container = new Container().GetChildContainer(x => x.For<DisposableObject>().Use<DisposableObject>()))
			{
				o = container.Get<DisposableObject>();
				o.Disposed.Should().BeFalse();
			}
			o.Disposed.Should().BeTrue();
		}

		private class ParentObject
		{
			public ParentObject(string text)
			{
				Text = text;
			}

			public string Text { get; private set; }
		}

		private class DisposableObject : IDisposable
		{
			public bool Disposed { get; private set; }

			public void Dispose()
			{
				Disposed = true;
			}
		}
	}
}