using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class TypeExtensions_IsConcreteClassClosing
	{
		[Theory, ClassData(typeof(TestData))]
		public void should_determine_if_class_is_concrete_and_closes_genericTypeDefinition(Type type, Type typeDefinition, bool expected, Type genericType)
		{
			var reason = string.Format("{0} {1} concrete class closing {2}", GetName(type), expected ? "is" : "is not", GetName(typeDefinition));
			Type outType;
			var result = type.IsConcreteClassClosing(typeDefinition, out outType);
			result.ShouldBe(expected, reason);
			outType.ShouldBe(genericType);
		}

		static string GetName(Type type)
		{
			var types = type.GetGenericArguments();
			return types.Length == 0
				? type.Name
				: type.Name.Replace("`" + types.Length, string.Format("<{0}>", string.Join(", ", types.Select(GetName))));
		}

		private interface IInterface<T> { }
		private class Implementation<T> : IInterface<T> { }
		private class ImplementationInt : Implementation<int> { }

		private class TestData : IEnumerable<object[]>
		{
			public IEnumerator<object[]> GetEnumerator()
			{
				var typeDefinitions = new[]
				{
					typeof (IInterface<>),
					typeof (Implementation<>),
				};
				var types = new[]
				{
					typeof (IInterface<>),
					typeof (Implementation<>),
					typeof (Implementation<int>)
				};

				foreach (var type in types)
					foreach (var typeDefinition in typeDefinitions)
					{
						var item = Valid.FirstOrDefault(x => x.type == type && x.definition == typeDefinition);
						var isValid = item != null;
						yield return new object[] { type, typeDefinition, isValid, isValid ? item.generic : default(Type) };
					}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			private static readonly dynamic[] Valid = new[]
			{
				new {type = typeof (Implementation<int>), definition = typeof (IInterface<>), generic = typeof (IInterface<int>)},
				new {type = typeof (Implementation<int>), definition = typeof (Implementation<>), generic = typeof (Implementation<int>)},
				new {type = typeof (ImplementationInt), definition = typeof (IInterface<>), generic = typeof (IInterface<int>)},
				new {type = typeof (ImplementationInt), definition = typeof (Implementation<>), generic = typeof (Implementation<int>)}
			};
		}
	}
}