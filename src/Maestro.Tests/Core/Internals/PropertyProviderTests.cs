using Maestro.Internals;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.Internals
{
	public class PropertyProviderTests
	{
		[Fact]
		public void Should_get_public_and_internal_properties()
		{
			var provider = new PropertyProvider();

			provider.GetProperty(typeof(AccessModifyer), nameof(AccessModifyer.PublicGetInternalSet)).ShouldNotBeNull();
			provider.GetProperty(typeof(AccessModifyer), nameof(AccessModifyer.PublicGetPrivateSet)).ShouldBeNull();
			provider.GetProperty(typeof(AccessModifyer), nameof(AccessModifyer.PublicGetProtectedSet)).ShouldBeNull();
			provider.GetProperty(typeof(AccessModifyer), nameof(AccessModifyer.PublicGetProtectedInternalSet)).ShouldBeNull();
			provider.GetProperty(typeof(AccessModifyer), nameof(AccessModifyer.PublicGetPublicSet)).ShouldNotBeNull();
		}

		[Fact]
		public void Should_get_instance_properties()
		{
			var provider = new PropertyProvider();

			provider.GetProperty(typeof(InstanceVsStatic), nameof(InstanceVsStatic.Instance)).ShouldNotBeNull();
			provider.GetProperty(typeof(InstanceVsStatic), nameof(InstanceVsStatic.Static)).ShouldBeNull();
		}

		[Fact]
		public void Should_get_properties_with_setter()
		{
			var provider = new PropertyProvider();

			provider.GetProperty(typeof(GetOrSet), nameof(GetOrSet.Get)).ShouldBeNull();
			provider.GetProperty(typeof(GetOrSet), nameof(GetOrSet.GetSet)).ShouldNotBeNull();
			provider.GetProperty(typeof(GetOrSet), nameof(GetOrSet.Set)).ShouldNotBeNull();
		}

		private class AccessModifyer
		{
			public int PublicGetInternalSet { get; internal set; }
			public int PublicGetPrivateSet { get; private set; }
			public int PublicGetProtectedSet { get; protected set; }
			public int PublicGetProtectedInternalSet { get; protected internal set; }
			public int PublicGetPublicSet { get; set; }
		}

		private class InstanceVsStatic
		{
			public int Instance { get; set; }
			public static int Static { get; set; }
		}

		private class GetOrSet
		{
			private int _i;

			public int Get { get; }
			public int GetSet { get; set; }

			public int Set
			{
				set { _i = value; }
			}
		}
	}
}