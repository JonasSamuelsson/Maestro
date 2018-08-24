using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Maestro.Tests.OldTests
{
	public class TypeExtensions_IsConcreteClassOf
	{
		[Theory, ClassData(typeof(TestData))]
		public void should_determine_if_type_is_concrete_sub_class_of_basetype(Type type, Type basetype, bool result, Type expectedGenericType)
		{
			var reason = $"{(result ? "Expected" : "Did not expect")} {GetName(type)} to be concrete class of {GetName(basetype)} with generic type definition {expectedGenericType?.Name ?? "null"}.";
			type.IsConcreteClassOf(basetype, out Type genericType).ShouldBe(result, reason);
			genericType.ShouldBe(expectedGenericType, reason);
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
		private interface IInterfaceOfInt : IInterface<int> { }
		private class Class : IInterface { }
		private class Class<T> : Class, IInterface<T> { }
		private class ClassOfInt : Class<int>, IInterfaceOfInt { }
		private abstract class AbstractClass : Class { }
		private class SubClass<T> : Class<T> { }

		public class TestData : IEnumerable<object[]>
		{
			public IEnumerator<object[]> GetEnumerator()
			{
				var types = new[]
				{
					typeof(IInterface),
					typeof(IInterface<>),
					typeof(IInterfaceOfInt),
					typeof(Class),
					typeof(Class<>),
					typeof(ClassOfInt),
					typeof(AbstractClass),
					typeof(SubClass<>)
				};

				var enumerable = from type in types
									  from baesType in types
									  let item = ConcreteClasses.SingleOrDefault(x => x.BaseType == baesType && x.ConcreteType == type)
									  select new object[] { type, baesType, item != null, item?.GenericType };

				return enumerable.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			private static readonly IEnumerable<Item> ConcreteClasses = new[]
			{
				new Item {BaseType = typeof(IInterface),      ConcreteType = typeof(Class),      GenericType = null                   },
				new Item {BaseType = typeof(IInterface),      ConcreteType = typeof(ClassOfInt), GenericType = null                   },
				new Item {BaseType = typeof(IInterface<>),    ConcreteType = typeof(Class<>),    GenericType = null                   },
				new Item {BaseType = typeof(IInterface<>),    ConcreteType = typeof(ClassOfInt), GenericType = typeof(IInterface<int>)},
				new Item {BaseType = typeof(IInterface<>),    ConcreteType = typeof(SubClass<>), GenericType = null                   },
				new Item {BaseType = typeof(IInterfaceOfInt), ConcreteType = typeof(ClassOfInt), GenericType = null                   },
				new Item {BaseType = typeof(Class),           ConcreteType = typeof(Class),      GenericType = null                   },
				new Item {BaseType = typeof(Class),           ConcreteType = typeof(ClassOfInt), GenericType = null                   },
				new Item {BaseType = typeof(Class<>),         ConcreteType = typeof(Class<>),    GenericType = null                   },
				new Item {BaseType = typeof(Class<>),         ConcreteType = typeof(ClassOfInt), GenericType = typeof(Class<int>)     },
				new Item {BaseType = typeof(Class<>),         ConcreteType = typeof(SubClass<>), GenericType = null                   },
				new Item {BaseType = typeof(ClassOfInt),      ConcreteType = typeof(ClassOfInt), GenericType = null                   },
				new Item {BaseType = typeof(SubClass<>),      ConcreteType = typeof(SubClass<>), GenericType = null                   }
			};
		}

		class Item
		{
			public Type BaseType { get; set; }
			public Type ConcreteType { get; set; }
			public Type GenericType { get; set; }
		}
	}
}