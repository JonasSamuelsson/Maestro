﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_array_dependencies
	{
		[Fact]
		public void should_use_all_registered_instences_of_the_enumerated_type()
		{
			var container = new Container(x =>
			{
				x.For<TypeWithArrayOfObjectDependency>().Use<TypeWithArrayOfObjectDependency>();
				x.Add<object>().Use<EventArgs>();
				x.Add<object>().Use<Exception>();
			});

			var instance = container.Get<TypeWithArrayOfObjectDependency>();

			instance.Objects.Should().HaveCount(2);
			instance.Objects.Should().Contain(x => x.GetType() == typeof(EventArgs));
			instance.Objects.Should().Contain(x => x.GetType() == typeof(Exception));
		}

		[Fact]
		public void should_use_empty_enumerable_if_enumerated_type_is_not_registered_and_not_value_type()
		{
			var container = new Container(x => x.For<TypeWithArrayOfObjectDependency>().Use<TypeWithArrayOfObjectDependency>());

			var instance = container.Get<TypeWithArrayOfObjectDependency>();

			instance.Objects.Should().HaveCount(0);
		}

		private class TypeWithArrayOfObjectDependency
		{
			public TypeWithArrayOfObjectDependency(object[] objects) { Objects = objects; }
			public IEnumerable<object> Objects { get; private set; }
		}

		[Fact]
		public void should_resolve_value_type_arrays_if_the_array_is_registered()
		{
			var dependency = new[] { 5 };
			var container = new Container(x => x.For<int[]>().Use(dependency));

			var instance = container.Get<TypeWithArrayOfValueTypeDependency>();

			instance.Ints.Should().BeEquivalentTo(dependency);
		}

		[Fact]
		public void should_resolve_value_type_arrays_if_the_element_type_is_registered()
		{
			var elementValue = 9;
			var container = new Container(x => x.For<int>().Use(elementValue));

			var instance = container.Get<TypeWithArrayOfValueTypeDependency>();

			instance.Ints.Should().BeEquivalentTo(new[] { elementValue });
		}

		[Fact]
		public void should_not_resolve_value_type_arays_if_array_type_and_element_type_is_unregistered()
		{
			var container = new Container();

			var instance = container.Get<TypeWithArrayOfValueTypeDependency>();

			instance.Ints.Should().BeNull();
		}

		private class TypeWithArrayOfValueTypeDependency
		{
			public TypeWithArrayOfValueTypeDependency() { }
			public TypeWithArrayOfValueTypeDependency(int[] ints) { Ints = ints; }
			public IEnumerable<int> Ints { get; private set; }
		}
	}
}