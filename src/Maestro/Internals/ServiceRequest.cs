using System;

namespace Maestro.Internals
{
	internal struct ServiceRequest
	{
		public ServiceRequest(Type type, string name)
		{
			Type = type;
			Name = name;
		}

		public Type Type { get; }
		public string Name { get; }

		public bool Equals(ServiceRequest other)
		{
			return Type == other.Type && string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			return obj is ServiceRequest && Equals((ServiceRequest)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Type?.GetHashCode() ?? 0) * 397) ^ (Name?.GetHashCode() ?? 0);
			}
		}
	}
}