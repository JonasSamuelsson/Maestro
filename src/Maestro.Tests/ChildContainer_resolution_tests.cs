﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class ChildContainer_resolution_tests
	{
		[Fact]
		public void should_prefer_generic_child_container_config_over_specific_parent_config()
		{
			var parentContainer = new Container(x => x.For<ICollection<object>>().Use<Collection<object>>());
			var childContainer = parentContainer.GetChildContainer(x => x.For(typeof(ICollection<>)).Use(typeof(List<>)));

			var collection = childContainer.Get<ICollection<object>>();

			collection.ShouldBeOfType<List<object>>();
		}

		[Fact]
		public void GetAll_should_get_instances_from_parent_container_if_name_is_different()
		{
			var parentContainer = new Container(x =>
			{
				x.For<object>().Use("default parent");
				x.For<object>().Add("parent");
			});

			var childContainer = parentContainer.GetChildContainer(x =>
			{
				x.For<object>().Use("default child");
				x.For<object>().Add("child");
			});

			childContainer.GetAll<object>().ShouldBe(new[] { "default child", "child", "parent" });
		}
	}
}