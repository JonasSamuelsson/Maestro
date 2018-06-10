using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Maestro.Tests.OldTests
{
	public class TypeExtensions_IsConcreteClassClosing
	{
		[Theory, ClassData(typeof(TestData))]
		public void should_determine_if_class_is_concrete_and_closes_genericTypeDefinition(Type type, Type typeDefinition, bool expected, Type genericType)
		{
			var result = type.IsConcreteClassClosing(typeDefinition, out var genericTypes);
			result.ShouldBe(expected, () => $"{GetName(type)} {(expected ? "is" : "is not")} concrete class closing {GetName(typeDefinition)}");
			if (genericType == null) return;
			genericTypes.ShouldContain(genericType);
		}

		[Fact]
		public void should_support_multiple_generic_interface_implementations_per_class()
		{
			typeof(MultiImplementation).IsConcreteClassClosing(typeof(IInterface1<>), out var types).ShouldBeTrue();
			types.ShouldBe(new[] { typeof(IInterface1<int>), typeof(IInterface1<object>) });
		}

		private static string GetName(Type type)
		{
			var types = type.GetGenericArguments();
			return types.Length == 0
				? type.Name
				: type.Name.Replace("`" + types.Length, $"<{string.Join(", ", types.Select(GetName))}>");
		}

		private interface IInterface1<T> { }
		private interface IInterface2<T> : IInterface1<T> { }
		private class Implementation1<T> : IInterface2<T> { }
		private class Implementation2<T> : Implementation1<T> { }
		private class ImplementationInt : Implementation2<int> { }

		private class TestData : IEnumerable<object[]>
		{
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public IEnumerator<object[]> GetEnumerator()
			{
				return GetData().GetEnumerator();
			}

			private IEnumerable<object[]> GetData()
			{
				var types = new[]
				{
					typeof(IInterface1<>),
					typeof(IInterface1<int>),
					typeof(IInterface2<>),
					typeof(IInterface2<int>),
					typeof(Implementation1<>),
					typeof(Implementation1<int>),
					typeof(Implementation2<>),
					typeof(Implementation2<int>),
					typeof(ImplementationInt)
				};

				var validCombinations = new[]
				{
					new { gtd = typeof(IInterface1<>),     t = typeof(Implementation1<int>), gt = typeof(IInterface1<int>)     },
					new { gtd = typeof(IInterface1<>),     t = typeof(Implementation2<int>), gt = typeof(IInterface1<int>)     },
					new { gtd = typeof(IInterface1<>),     t = typeof(ImplementationInt),    gt = typeof(IInterface1<int>)     },
					new { gtd = typeof(IInterface2<>),     t = typeof(Implementation1<int>), gt = typeof(IInterface2<int>)     },
					new { gtd = typeof(IInterface2<>),     t = typeof(Implementation2<int>), gt = typeof(IInterface2<int>)     },
					new { gtd = typeof(IInterface2<>),     t = typeof(ImplementationInt),    gt = typeof(IInterface2<int>)     },
					new { gtd = typeof(Implementation1<>), t = typeof(Implementation1<int>), gt = typeof(Implementation1<int>) },
					new { gtd = typeof(Implementation1<>), t = typeof(Implementation2<int>), gt = typeof(Implementation1<int>) },
					new { gtd = typeof(Implementation1<>), t = typeof(ImplementationInt),    gt = typeof(Implementation1<int>) },
					new { gtd = typeof(Implementation2<>), t = typeof(Implementation2<int>), gt = typeof(Implementation2<int>) },
					new { gtd = typeof(Implementation2<>), t = typeof(ImplementationInt),    gt = typeof(Implementation2<int>) }
				};

				return from type in types
						 from genericTypeDefinition in types
						 where genericTypeDefinition.IsGenericTypeDefinition
						 let validCombination = validCombinations.SingleOrDefault(x => x.t == type && x.gtd == genericTypeDefinition)
						 let isValid = validCombination != null
						 let genericType = isValid ? validCombination.gt : null
						 select new object[] { type, genericTypeDefinition, isValid, genericType };
			}
		}

		private class MultiImplementation : IInterface1<int>, IInterface1<object> { }
	}
}