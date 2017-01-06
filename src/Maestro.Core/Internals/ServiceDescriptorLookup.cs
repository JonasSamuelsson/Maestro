using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Utils;

namespace Maestro.Internals
{
	internal class ServiceDescriptorLookup : IDisposable
	{
		private readonly ThreadSafeDictionary<Type, ServiceFamily> _serviceFamilies = new ThreadSafeDictionary<Type, ServiceFamily>();

		internal const string DefaultName = "";

		public static string GetRandomName()
		{
			return Guid.NewGuid().ToString();
		}

		public bool Add(ServiceDescriptor serviceDescriptor, bool throwIfDuplicate = true)
		{
			var serviceFamily = _serviceFamilies.GetOrAdd(serviceDescriptor.Type, _ => new ServiceFamily());

			try
			{
				serviceFamily.Services.Add(serviceDescriptor.Name, serviceDescriptor);
				return true;
			}
			catch (ArgumentException)
			{
				if (throwIfDuplicate) throw;
				return false;
			}
		}

		public bool TryGetServiceDescriptor(Type type, string name, out ServiceDescriptor serviceDescriptor)
		{
			ServiceFamily serviceFamily;

			if (_serviceFamilies.TryGet(type, out serviceFamily))
			{
				if (serviceFamily.Services.TryGetValue(name, out serviceDescriptor))
				{
					return true;
				}
			}

			Type genericTypeDefinition;
			Type[] genericArguments;
			if (Reflector.IsGeneric(type, out genericTypeDefinition, out genericArguments))
			{
				if (TryGetServiceDescriptor(genericTypeDefinition, name, out serviceDescriptor))
				{
					serviceDescriptor = serviceDescriptor.MakeGeneric(genericArguments);
					Add(serviceDescriptor);
					return true;
				}
			}

			serviceDescriptor = null;
			return false;
		}

		public bool TryGetServiceDescriptors(Type type, out IEnumerable<ServiceDescriptor> serviceDescriptors)
		{
			ServiceFamily serviceFamily;
			var result = new List<ServiceDescriptor>();

			if (_serviceFamilies.TryGet(type, out serviceFamily))
			{
				if (serviceFamily.Services.Count != 0)
				{
					result.AddRange(serviceFamily.Services.Values);
				}
			}

			Type genericTypeDefinition;
			Type[] genericArguments;
			if (Reflector.IsGeneric(type, out genericTypeDefinition, out genericArguments))
			{
				if (TryGetServiceDescriptors(genericTypeDefinition, out serviceDescriptors))
				{
					serviceDescriptors = serviceDescriptors
						.Where(x => result.All(y => x.CorrelationId != y.CorrelationId))
						.Select(x => x.MakeGeneric(genericArguments))
						.ToList();
					foreach (var serviceDescriptor in serviceDescriptors) Add(serviceDescriptor);
					result.AddRange(serviceDescriptors);
				}
			}

			if (result.Count != 0)
			{
				serviceDescriptors = result;
				return true;
			}

			serviceDescriptors = null;
			return false;
		}

		public void Dispose()
		{
			// todo
		}
	}
}