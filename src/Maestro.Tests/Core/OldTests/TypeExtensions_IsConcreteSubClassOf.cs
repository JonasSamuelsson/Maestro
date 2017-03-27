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
			var reason = $"{(expected ? "Expected" : "Did not expect")} {GetName(type)} to be concrete sub class of {GetName(basetype)}";
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
		private interface IInterfaceOfInt : IInterface<int> { }
		private class Class : IInterface { }
		private class Class<T> : Class, IInterface<T> { }
		private class ClassOfInt : Class<int>, IInterfaceOfInt { }
		private abstract class AbstractClass : Class { }

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
					typeof(AbstractClass)
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
				new {basetype = typeof(IInterface),      concreteSubTypes = new[] {typeof(Class), typeof(ClassOfInt)}},
				new {basetype = typeof(IInterface<>),    concreteSubTypes = new[] {typeof(Class<>)}},
				new {basetype = typeof(IInterfaceOfInt), concreteSubTypes = new[] {typeof(ClassOfInt)}},
				new {basetype = typeof(Class),           concreteSubTypes = new[] {typeof(ClassOfInt)}}
			}.ToDictionary(x => x.basetype, x => x.concreteSubTypes.AsEnumerable());
		}
	}
}