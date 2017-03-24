using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core
{
	public class TypeExtensions_IsConcreteSubClassOf
	{
		[Theory, ClassData(typeof(TestData))]
		public void should_determine_if_type_is_concrete_sub_class_of_basetype(Type type, Type basetype, bool expected)
		{
			var reason = $"{GetName(type)} {(expected ? "is" : "is not")} concrete sub class of {GetName(basetype)}";
			type.IsConcreteSubClassOf(basetype).ShouldBe(expected, reason);
		}

		static string GetName(Type type)
		{
			var types = type.GetGenericArguments();
			return types.Length == 0
				? type.Name
				: type.Name.Replace("`" + types.Length, $"<{string.Join(", ", types.Select(GetName))}>");
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
					typeof (Class<int, int>),
					typeof (AbstractClass)
				};

				IEnumerable<Type> concreteSubClasses = null;

				var enumerable = from type in types
									  from basetype in types
									  let hasConcreteSubClasses = ConcreteSubClasses.TryGetValue(basetype, out concreteSubClasses)
									  select new object[] { type, basetype, hasConcreteSubClasses && concreteSubClasses.Contains(type) };

				return enumerable.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			private static readonly Dictionary<Type, IEnumerable<Type>> ConcreteSubClasses = new[]
			{
				new {type = typeof (Implementation), basetype = typeof (IInterface)},
				new {type = typeof (Implementation<int>), basetype = typeof (IInterface)},
				new {type = typeof (Implementation<int>), basetype = typeof (IInterface<int>)},
				new {type = typeof (Implementation<int,int>), basetype = typeof (IInterface)},
				new {type = typeof (Implementation<int,int>), basetype = typeof (IInterface<int>)},
				new {type = typeof (Implementation<int,int>), basetype = typeof (IInterface<int,int>)},
				new {type = typeof (Class<int>), basetype = typeof (Class)},
				new {type = typeof (Class<int,int>), basetype = typeof (Class)},
				new {type = typeof (Class<int,int>), basetype = typeof (Class<int>)},
			}.GroupBy(x => x.basetype).ToDictionary(x => x.Key, x => x.Select(y => y.type).ToList().AsEnumerable());
		}
	}
}