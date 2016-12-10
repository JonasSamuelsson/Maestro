﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_array_dependencies
	{
		[Todo]
		public void should_use_all_registered_instances_of_the_enumerated_type()
		{
			var container = new Container(x =>
			{
				x.Service<TypeWithArrayOfObjectDependency>().Use.Type<TypeWithArrayOfObjectDependency>();
				x.Services<object>().Add.Type<object>();
				x.Services<object>().Add.Type<EventArgs>();
			});

			var instance = container.GetService<TypeWithArrayOfObjectDependency>();

			instance.Objects.Should().HaveCount(2);
			instance.Objects.Should().Contain(x => x.GetType() == typeof(object));
			instance.Objects.Should().Contain(x => x.GetType() == typeof(EventArgs));
		}

		[Todo]
		public void should_use_empty_enumerable_if_enumerated_type_is_not_registered_and_not_value_type()
		{
			var container = new Container(x => x.Service<TypeWithArrayOfObjectDependency>().Use.Type<TypeWithArrayOfObjectDependency>());

			var instance = container.GetService<TypeWithArrayOfObjectDependency>();

			instance.Objects.Should().HaveCount(0);
		}

		private class TypeWithArrayOfObjectDependency
		{
			public TypeWithArrayOfObjectDependency(object[] objects) { Objects = objects; }
			public IEnumerable<object> Objects { get; private set; }
		}

		[Fact]
		public void should_not_resolve_value_type_arrays_unless_the_array_type_is_registered()
		{
			var array = new[] { 1, 2, 3 };
			var container = new Container();

			container.GetService<TypeWithArrayOfValueTypeDependency>().Ints.Should().BeNull();

			container.Configure(x => x.Services<int>().Add.Instance(0));
			container.GetService<TypeWithArrayOfValueTypeDependency>().Ints.Should().BeNull();

			container.Configure(x => x.Service<int[]>().Use.Instance(array));
			container.GetService<TypeWithArrayOfValueTypeDependency>().Ints.Should().BeEquivalentTo(array);
		}

		private class TypeWithArrayOfValueTypeDependency
		{
			public TypeWithArrayOfValueTypeDependency() { }
			public TypeWithArrayOfValueTypeDependency(int[] ints) { Ints = ints; }
			public IEnumerable<int> Ints { get; private set; }
		}
	}
}