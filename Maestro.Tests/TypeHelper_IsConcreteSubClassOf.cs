using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit.Extensions;

namespace Maestro.Tests
{
	public class TypeHelper_IsConcreteSubClassOf
	{
		[Theory, ClassData(typeof(TestData))]
		public void should_determine_if_type_is_concrete_sub_class_of_basetype(Type @from, Type to, bool expected)
		{
			var reason = string.Format("{0} {1} cast to {2}", GetName(@from), expected ? "can" : "can not", GetName(to));
			@from.IsConcreteSubClassOf(to).Should().Be(expected, reason);
		}

		static string GetName(Type type)
		{
			var types = type.GetGenericArguments();
			return types.Length == 0
				? type.Name
				: type.Name.Replace("`" + types.Length, string.Format("<{0}>", string.Join(", ", types.Select(GetName))));
		}

		private interface IInterface { }
		private interface IInterface<T> : IInterface { }
		private interface IInterface<T1, T2> : IInterface<T1> { }

		private class Implementation : IInterface { }
		private class Implementation<T> : IInterface<T> { }
		private class Implementation<T1, T2> : IInterface<T1, T2> { }

		private class Class { }
		private class Class<T> : Class { }
		private class Class<T1, T2> : Class<T1> { }
		private abstract class AbstractClass : Class { }
		private class GenericClass1<T1, T2> : Class<T1, int> { }
		private class GenericClass2<T1, T2> : Class<T1, T1> { }
		private class GenericClassNotFollowingTypeArgNamingConvention<U> : Class<U> { }

		public class TestData : IEnumerable<object[]>
		{
			public IEnumerator<object[]> GetEnumerator()
			{
				var types = new[]
				{
					typeof (IInterface),
					typeof (IInterface<>),
					typeof (IInterface<int>),
					typeof (IInterface<,>),
					typeof (IInterface<int, int>),
					typeof (Implementation),
					typeof (Implementation<>),
					typeof (Implementation<int>),
					typeof (Implementation<,>),
					typeof (Implementation<int, int>),
					typeof (Class),
					typeof (Class<>),
					typeof (Class<int>),
					typeof (Class<,>),
					typeof (Class<int, int>)
				};

				IEnumerable<Type> enumerable;
				var objectses = from type in types
									 from basetype in types
									 select new object[] { type, basetype, ConcreteSubClasses.TryGetValue(basetype, out enumerable) && enumerable.Contains(type) };

				var objects = new[]
				{
					new object[] {typeof (AbstractClass), typeof (Class), false},
					new object[] {typeof (GenericClass1<,>), typeof (Class<,>), false},
					new object[] {typeof (GenericClass2<,>), typeof (Class<,>), false},
					new object[] {typeof (GenericClassNotFollowingTypeArgNamingConvention<>), typeof (Class<>), true},
				};

				return objectses.Concat(objects).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			private static readonly Dictionary<Type, IEnumerable<Type>> ConcreteSubClasses = new[]
			{
				new {type = typeof (Implementation), basetype = typeof (IInterface)},
				new {type = typeof (Implementation<>), basetype = typeof (IInterface<>)},
				new {type = typeof (Implementation<int>), basetype = typeof (IInterface)},
				new {type = typeof (Implementation<int>), basetype = typeof (IInterface<int>)},
				new {type = typeof (Implementation<,>), basetype = typeof (IInterface<,>)},
				new {type = typeof (Implementation<int,int>), basetype = typeof (IInterface)},
				new {type = typeof (Implementation<int,int>), basetype = typeof (IInterface<int>)},
				new {type = typeof (Implementation<int,int>), basetype = typeof (IInterface<int,int>)},
				new {type = typeof (Class<int>), basetype = typeof (Class)},
				new {type = typeof (Class<int,int>), basetype = typeof (Class)},
				new {type = typeof (Class<int,int>), basetype = typeof (Class<int>)},
				new {type = typeof (GenericClassNotFollowingTypeArgNamingConvention<>), basetype = typeof (Class<>)}
			}.GroupBy(x => x.basetype).ToDictionary(x => x.Key, x => x.Select(y => y.type).ToList().AsEnumerable());
		}
	}
}